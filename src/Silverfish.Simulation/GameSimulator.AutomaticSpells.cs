using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private void CastWhenDrawn(Stack<Work> work, CastDrawnSpell spell)
        {
            if (_state.LocationOf(spell.Source) != new EntityLocation(spell.Player, GameZone.Hand))
                return;

            SpendResolutionStep();
            var card = _rules.Card(_state.Entities[spell.Source]);
            var (target, effects, _) = SelectAutomaticSpellEffects(_state.Entities[spell.Source], card);
            var player = _state.Players[spell.Player];
            var armsSecret = card.IsSecret && CanArmSecret(_state, player, card.Definition.CardId);
            _state = _state.Move(spell.Source, new(spell.Player, armsSecret ? GameZone.Secret : GameZone.Play));
            _events.Add(new(SimulationEventKind.SpellCastWhenDrawn, spell.Player, spell.Source, target));
            CountSpellCast(spell.Player);
            AddOverload(spell.Player, spell.Source, card.Overload);

            if (armsSecret)
                _events.Add(new(SimulationEventKind.SecretArmed, spell.Player, spell.Source));
            else
                work.Push(new FinishDrawnSpell(spell.Player, spell.Source));
            PushEffects(work, new(spell.Source, spell.Player, target), effects);
        }

        private void CastRandomSpells(Stack<Work> work, EffectContext context, CastRandomSpellEffect effect)
        {
            var count = effect.ForEachSpellCast ? _state.Players[context.Controller].SpellsCastThisGame : 1;
            var candidates = _rules.GeneratedCardCandidates(new()
            {
                Kind = EntityKind.Spell, MinimumPrintedCost = effect.MinimumPrintedCost
            });
            if (count > 0 && !candidates.IsEmpty)
                work.Push(new CastFromPool(context.Controller, context.Source, candidates, count));
        }

        private void CastFromPoolOnce(Stack<Work> work, CastFromPool cast)
        {
            if (!_state.Players.Values.All(player => IsLivingCharacter(player.Hero)) || !IsLivingCharacter(cast.Source))
                return;
            var card = _rules.GetCard(cast.CardIds[ChooseIndex(cast.CardIds.Length)]);
            var player = _state.Players[cast.Player];
            var source = new EntityId(_state.NextEntity);
            if (cast.Remaining > 1)
                work.Push(cast with { Remaining = cast.Remaining - 1 });

            var entity = new GameEntity(source, card.Definition.CardId, player.Id, player.Id);
            _state = _state.AddToSetAside(entity);
            _state = _state.Move(source, new(player.Id, GameZone.Play));
            var (target, effects, branch) = SelectAutomaticSpellEffects(entity, card);
            _events.Add(new(SimulationEventKind.SpellCastAutomatically, player.Id, source, target));
            var listeners = CaptureListeners();
            RunTriggers(new(TriggerTiming.SpellCounterWindow, player.Id, source), listeners);
            if (!CanCounterSpell(source))
                return;
            if (target is EntityId proposed && _rules.Card(_state.Entities[proposed]).Definition.Kind == EntityKind.Minion)
                target = ResolveSpellTargetProposal(player.Id, proposed);

            CountSpellCast(player.Id);
            RunTriggers(new(TriggerTiming.BeforeSpellResolved, player.Id, source, Amount: card.Definition.PrintedCost), listeners);
            if (target is EntityId selected)
                RunTriggers(new(TriggerTiming.TargetedBySpell, player.Id, selected), listeners);
            AddOverload(player.Id, source, card.Overload);
            if (card.EffectPending)
                _events.Add(new(SimulationEventKind.AutomaticSpellEffectPending, player.Id, source));
            else if (card.IsSecret && CanArmSecret(_state, _state.Players[player.Id], card.Definition.CardId))
            {
                _state = _state.Move(source, new(player.Id, GameZone.Secret));
                _events.Add(new(SimulationEventKind.SecretArmed, player.Id, source));
            }
            else
            {
                ResolveEffects(new(source, player.Id, target), effects);
                if (ChoiceRule(card, branch) is DiscoverCardChoice discover &&
                    (discover.Condition is null || MeetsCondition(new(source, player.Id, target), discover.Condition)))
                    ResolveAutomaticDiscover(player.Id, source, discover);
            }
            if (_state.LocationOf(source).Zone == GameZone.Play)
                _state = _state.Move(source, new(player.Id, GameZone.Graveyard));
            ResolveDeaths();
            RunTriggers(new(TriggerTiming.AfterSpellResolved, player.Id, source, Amount: card.Definition.PrintedCost,
                Defender: target, PlayedBranch: branch), listeners);
            ResolveSpellCastRewards(player.Id, source);
        }

        private void ResolveAutomaticDiscover(PlayerId player, EntityId source, DiscoverCardChoice rule)
        {
            for (DiscoverCardChoice? round = rule; round is not null; round = round.Then)
            {
                var options = _rules.PlayChoiceCandidates(_state, player, source, round);
                if (!options.IsEmpty)
                    ResolveDiscoveredCard(player, source, options[ChooseIndex(options.Length)], round);
            }
        }

        private (EntityId? Target, ImmutableArray<CardEffect> Effects, string? Branch) SelectAutomaticSpellEffects(GameEntity source, SimulationCard card)
        {
            var eligible = _rules.Branches(_state, source, card, false)
                .Where(branch => !branch.Targets.IsEmpty)
                .ToArray();
            if (eligible.Length == 0)
                return (null, [], null);

            var selected = eligible[ChooseIndex(eligible.Length)];
            var target = selected.Targets[ChooseIndex(selected.Targets.Length)];
            var choice = selected.Choice is null ? null : PlayChoiceAtResolution(card, selected.Choice);
            return (target, selected.ResolvesEffects(target)
                ? choice?.Effects ?? card.OnPlay : [], selected.Choice);
        }
    }
}
