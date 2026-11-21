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
            var (target, effects) = SelectAutomaticSpellEffects(_state.Entities[spell.Source], card);
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

        private (EntityId? Target, ImmutableArray<CardEffect> Effects) SelectAutomaticSpellEffects(GameEntity source, SimulationCard card)
        {
            var eligible = _rules.Branches(_state, source, card, false)
                .Where(branch => !branch.Targets.IsEmpty)
                .ToArray();
            if (eligible.Length == 0)
                return (null, []);

            var selected = eligible[ChooseIndex(eligible.Length)];
            var target = selected.Targets[ChooseIndex(selected.Targets.Length)];
            var choice = selected.Choice is null ? null : card.Choices.Single(choice => choice.Id == selected.Choice);
            return (target, selected.ResolvesEffects(target)
                ? choice?.Effects ?? card.OnPlay : []);
        }
    }
}
