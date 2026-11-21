using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private int ModifiedEffectAmount(EffectContext context, int amount, bool damage)
        {
            var sourceKind = _rules.Card(_state.Entities[context.Source]).Definition.Kind;
            if (sourceKind is not (EntityKind.Spell or EntityKind.HeroPower))
                return amount;

            var multiplier = 1;
            foreach (var id in _rules.BoardMinions(_state, context.Controller))
            {
                var minion = _state.Entities[id];
                if (minion.Silenced || !IsLivingCharacter(id))
                    continue;

                var card = _rules.Card(minion);
                if (damage && sourceKind == EntityKind.Spell)
                    amount = checked(amount + card.SpellDamage);
                if (card.SpellAndHeroPowerMultiplier is int factor)
                    multiplier = checked(multiplier * factor);
            }

            return checked(Math.Max(0, amount) * multiplier);
        }
    }
}
