using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private bool HasHealingDealsDamage(PlayerId controller) =>
            _rules._hasHealingDealsDamage && _rules.BoardMinions(_state, controller).Any(id =>
            {
                var minion = _state.Entities[id];
                return !minion.Silenced && IsLivingCharacter(id) && _rules.Card(minion).HealingDealsDamage;
            });

        private int ModifiedEffectAmount(EffectContext context, int amount, bool damage)
        {
            var sourceKind = _rules.Card(_state.Entities[context.Source]).Definition.Kind;
            if (sourceKind is not (EntityKind.Spell or EntityKind.HeroPower))
                return amount;

            var effectiveDamage = damage || HasHealingDealsDamage(context.Controller);
            var multiplier = 1;
            foreach (var id in _rules.BoardMinions(_state, context.Controller))
            {
                var minion = _state.Entities[id];
                if (!IsLivingCharacter(id))
                    continue;

                var card = _rules.Card(minion);
                if (effectiveDamage && sourceKind == EntityKind.Spell)
                {
                    var bonus = (minion.Silenced ? 0 : card.SpellDamage) +
                        minion.Enchantments.Count(layer => layer.GrantedTag == GAME_TAG.SPELLPOWER);
                    amount = checked(amount + bonus);
                }
                if (!minion.Silenced && card.SpellAndHeroPowerMultiplier is int factor)
                    multiplier = checked(multiplier * factor);
            }

            return checked(Math.Max(0, amount) * multiplier);
        }
    }
}
