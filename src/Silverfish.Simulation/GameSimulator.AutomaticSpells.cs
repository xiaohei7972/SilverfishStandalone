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
            var (target, effects) = SelectAutomaticSpellEffects(spell.Player, card);
            var player = _state.Players[spell.Player];
            var armsSecret = card.IsSecret && CanArmSecret(_state, player, card.Definition.CardId);
            _state = _state.Move(spell.Source, new(spell.Player, armsSecret ? GameZone.Secret : GameZone.Play));
            _events.Add(new(SimulationEventKind.SpellCastWhenDrawn, spell.Player, spell.Source, target));
            AddOverload(spell.Player, spell.Source, card.Overload);

            if (armsSecret)
                _events.Add(new(SimulationEventKind.SecretArmed, spell.Player, spell.Source));
            else
                work.Push(new FinishDrawnSpell(spell.Player, spell.Source));
            PushEffects(work, new(spell.Source, spell.Player, target), effects);
        }

        private (EntityId? Target, ImmutableArray<CardEffect> Effects) SelectAutomaticSpellEffects(PlayerId player, SimulationCard card)
        {
            var occupiedSlots = _rules.BoardSlotCount(_state, player);
            var minions = _rules.MinionCount(_state, player);
            if (!_rules.MeetsPlayRequirements(_state, player, card, occupiedSlots, minions))
                return (null, []);

            (TargetRequirement Target, TargetFilter? Filter, ImmutableArray<CardEffect> Effects)[] options = card.Choices.IsEmpty
                ? [(card.Target, card.TargetFilter, card.OnPlay)]
                : card.Choices.Where(choice => MeetsPlayRequirements(_state, player, choice.Requirements, occupiedSlots))
                    .Select(choice => (choice.Target, choice.TargetFilter, choice.Effects)).ToArray();
            var eligible = options
                .Where(option => occupiedSlots < BoardLimit || !option.Effects.Any(effect => effect is TakeControlEffect))
                .Select(option => (Targets: _rules.PlayTargets(_state, player, option.Target, option.Filter).ToArray(), option.Effects))
                .Where(option => option.Targets.Length != 0)
                .ToArray();
            if (eligible.Length == 0)
                return (null, []);

            var selected = eligible[ChooseIndex(eligible.Length)];
            return (selected.Targets[ChooseIndex(selected.Targets.Length)], selected.Effects);
        }
    }
}
