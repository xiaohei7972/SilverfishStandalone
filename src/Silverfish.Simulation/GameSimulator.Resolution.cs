using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed class Resolution(GameSimulator rules, GameState state)
    {
        private readonly ImmutableArray<SimulationEvent>.Builder _events = ImmutableArray.CreateBuilder<SimulationEvent>();
        private GameState _state = state;
        private int _remainingSteps = rules._resolutionLimit;

        public SimulationResult Run(SimulationAction action)
        {
            switch (action.Kind)
            {
                case SimulationActionKind.EndTurn:
                    EndTurn(action.Actor);
                    break;
                case SimulationActionKind.PlayCard:
                    Play(action.Actor, action.Source!.Value, action.Target, action.Position);
                    break;
                case SimulationActionKind.Attack:
                    Attack(action.Actor, action.Source!.Value, action.Target!.Value);
                    break;
            }

            ResolveDeaths();
            FinishIfHeroesDied();
            return new SimulationResult(_state, _events.ToImmutable());
        }

        private void EndTurn(PlayerId outgoing)
        {
            _events.Add(new(SimulationEventKind.TurnEnded, outgoing));
            _state = _state.PassTurn();
            var next = _state.Players[_state.ActivePlayer];
            var crystals = Math.Min(next.ManaCrystals + 1, 10);
            _state = _state.SetResources(next.Id, crystals, crystals, next.Fatigue);
            foreach (var id in next.In(GameZone.Play))
            {
                var character = _state.Entities[id].Character!;
                _state = _state.UpdateCharacter(id, character with { AttacksThisTurn = 0, Exhausted = false });
            }

            _events.Add(new(SimulationEventKind.TurnStarted, next.Id));
            Draw(next.Id);
        }

        private void Draw(PlayerId playerId)
        {
            SpendResolutionStep();
            var player = _state.Players[playerId];
            var deck = player.In(GameZone.Deck);
            if (deck.IsEmpty)
            {
                var fatigue = checked(player.Fatigue + 1);
                _state = _state.SetResources(playerId, player.Mana, player.ManaCrystals, fatigue);
                _events.Add(new(SimulationEventKind.Fatigue, playerId, Target: player.Hero, Amount: fatigue));
                Damage(null, player.Hero, fatigue);
                return;
            }

            var drawn = deck[0];
            var burned = player.In(GameZone.Hand).Length >= HandLimit;
            _state = _state.Move(drawn, new(playerId, burned ? GameZone.Graveyard : GameZone.Hand));
            _events.Add(new(burned ? SimulationEventKind.CardBurned : SimulationEventKind.CardDrawn, playerId, drawn));
        }

        private void Play(PlayerId playerId, EntityId source, EntityId? target, int? position)
        {
            var card = rules.Card(_state.Entities[source]);
            var player = _state.Players[playerId];
            _state = _state.SetResources(playerId, player.Mana - card.Definition.PrintedCost, player.ManaCrystals, player.Fatigue);
            var index = position is int slot ? rules.BoardIndex(_state, playerId, slot) : (int?)null;
            _state = _state.Move(source, new(playerId, GameZone.Play), index);
            _events.Add(new(SimulationEventKind.CardPlayed, playerId, source, target));

            if (card.Definition.Kind == EntityKind.Minion)
            {
                _state = _state.UpdateCharacter(source, card.CreateCharacter());
                _events.Add(new(SimulationEventKind.Summoned, playerId, source));
            }

            ResolveEffects(source, playerId, target, card.OnPlay);
            if (card.Definition.Kind == EntityKind.Spell)
                _state = _state.Move(source, new(playerId, GameZone.Graveyard));
        }

        private void Attack(PlayerId player, EntityId source, EntityId target)
        {
            var attacker = _state.Entities[source].Character!;
            var defender = _state.Entities[target];
            var retaliation = rules.Card(defender).Definition.Kind == EntityKind.Minion ? defender.Character!.Attack : 0;
            _state = _state.UpdateCharacter(source, attacker with { AttacksThisTurn = attacker.AttacksThisTurn + 1 });
            _events.Add(new(SimulationEventKind.Attack, player, source, target));
            Damage(source, target, attacker.Attack);
            Damage(target, source, retaliation);
        }

        private void Damage(EntityId? source, EntityId target, int amount)
        {
            if (amount == 0 || _state.LocationOf(target).Zone != GameZone.Play)
                return;
            var entity = _state.Entities[target];
            var character = entity.Character!;
            if (character.DivineShield)
            {
                _state = _state.UpdateCharacter(target, character with { DivineShield = false });
                _events.Add(new(SimulationEventKind.ShieldBroken, entity.Controller, source, target));
                return;
            }

            var absorbed = Math.Min(character.Armor, amount);
            _state = _state.UpdateCharacter(target, character with
            {
                Armor = character.Armor - absorbed,
                Damage = checked(character.Damage + (amount - absorbed))
            });
            _events.Add(new(SimulationEventKind.Damage, entity.Controller, source, target, amount));
        }

        private void ResolveEffects(EntityId source, PlayerId controller, EntityId? target, ImmutableArray<CardEffect> effects)
        {
            foreach (var effect in effects)
            {
                SpendResolutionStep();
                switch (effect)
                {
                    case DamageEffect { Target: DamageTarget.Selected } damage:
                        Damage(source, target!.Value, damage.Amount);
                        break;
                    case DamageEffect { Target: DamageTarget.EnemyHero } damage:
                        Damage(source, _state.Players[_state.OpponentOf(controller)].Hero, damage.Amount);
                        break;
                    case DamageEffect damage:
                        var minions = _state.Players.Keys.SelectMany(player => rules.BoardMinions(_state, player))
                            .OrderBy(id => _state.Entities[id].PlayOrder).ToArray();
                        foreach (var id in minions)
                            Damage(source, id, damage.Amount);
                        break;
                    case DrawEffect draw:
                        for (var i = 0; i < draw.Count; i++)
                            Draw(controller);
                        break;
                    case SummonEffect summon:
                        Summon(controller, summon.CardId);
                        break;
                    case TakeControlEffect:
                        TakeControl(source, target!.Value, controller);
                        break;
                }
            }
        }

        private void Summon(PlayerId player, string cardId)
        {
            if (rules.MinionCount(_state, player) >= BoardLimit)
                return;
            var id = new EntityId(_state.NextEntity);
            var entity = new GameEntity(id, cardId, player, player) { Character = rules.GetCard(cardId).CreateCharacter() };
            _state = _state.AddToPlay(entity, _state.Players[player].In(GameZone.Play).Length);
            _events.Add(new(SimulationEventKind.Summoned, player, id));
        }

        private void TakeControl(EntityId source, EntityId target, PlayerId controller)
        {
            var entity = _state.Entities[target];
            if (entity.Controller == controller || rules.MinionCount(_state, controller) >= BoardLimit)
                return;
            _state = _state.Move(target, new(controller, GameZone.Play));
            var hasCharge = rules.Card(entity).Keywords.HasFlag(CardKeywords.Charge);
            _state = _state.UpdateCharacter(target, entity.Character! with { Exhausted = !hasCharge });
            _events.Add(new(SimulationEventKind.ControlChanged, controller, source, target));
        }

        private void ResolveDeaths()
        {
            while (true)
            {
                var dead = _state.Players.Values.SelectMany(player => player.In(GameZone.Play))
                    .Select(id => _state.Entities[id])
                    .Where(entity => entity.Character is { Health: <= 0 })
                    .OrderBy(entity => entity.PlayOrder).ToArray();
                if (dead.Length == 0)
                    return;

                foreach (var entity in dead)
                {
                    SpendResolutionStep();
                    _state = _state.Move(entity.Id, new(entity.Controller, GameZone.Graveyard));
                    _events.Add(new(SimulationEventKind.Died, entity.Controller, entity.Id));
                }

                foreach (var entity in dead)
                {
                    var effects = rules.Card(entity).Deathrattle;
                    if (effects.IsEmpty)
                        continue;
                    _events.Add(new(SimulationEventKind.Deathrattle, entity.Controller, entity.Id));
                    ResolveEffects(entity.Id, entity.Controller, null, effects);
                }
            }
        }

        private void FinishIfHeroesDied()
        {
            var survivors = _state.Players.Values
                .Where(player => _state.LocationOf(player.Hero).Zone == GameZone.Play).ToArray();
            if (survivors.Length == _state.Players.Count)
                return;
            PlayerId? winner = survivors.Length == 1 ? survivors[0].Id : null;
            _state = _state.Finish(winner);
            _events.Add(new(SimulationEventKind.MatchEnded, winner));
        }

        private void SpendResolutionStep()
        {
            if (_remainingSteps-- <= 0)
                throw new InvalidOperationException("The action exceeded the simulator's resolution budget.");
        }
    }
}
