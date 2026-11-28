using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private int SpellDamageBonus(GameEntity source, PlayerId recipient)
    {
        var card = Card(source);
        var bonus = source.Silenced ? 0 : card.BothPlayersSpellDamage.GetValueOrDefault();
        if (source.Controller == recipient)
            bonus = checked(bonus + (source.Silenced ? 0 : card.SpellDamage) +
                source.Enchantments.Count(layer => layer.GrantedTag == GAME_TAG.SPELLPOWER));
        return bonus;
    }

    private sealed partial class Resolution
    {
        private bool HasHealingDealsDamage(PlayerId controller) =>
            _state.Players[controller].HealingDealsDamageThisTurn is not null ||
            _rules._hasHealingDealsDamage && _rules.BoardMinions(_state, controller).Any(id =>
            {
                var minion = _state.Entities[id];
                return !minion.Silenced && IsLivingCharacter(id) && _rules.Card(minion).HealingDealsDamage;
            });

        private int ModifiedEffectAmount(EffectContext context, int amount, bool damage)
        {
            var source = _rules.Card(_state.Entities[context.Source]);
            var sourceKind = source.Definition.Kind;
            if (sourceKind is not (EntityKind.Spell or EntityKind.HeroPower))
                return amount;

            var effectiveDamage = damage || HasHealingDealsDamage(context.Controller);
            var multiplier = 1;
            foreach (var id in _state.Players.Keys.SelectMany(player => _rules.BoardMinions(_state, player)))
            {
                var minion = _state.Entities[id];
                if (!IsLivingCharacter(id))
                    continue;

                var card = _rules.Card(minion);
                var friendly = minion.Controller == context.Controller;
                if (effectiveDamage && sourceKind == EntityKind.Spell)
                {
                    amount = checked(amount + _rules.SpellDamageBonus(minion, context.Controller) * (source.SpellDamageBonusMultiplier ?? 1));
                }
                if (friendly && !minion.Silenced && effectiveDamage && sourceKind == EntityKind.HeroPower)
                    amount = checked(amount + card.FriendlyHeroPowerDamage.GetValueOrDefault());
                if (friendly && !minion.Silenced && card.SpellAndHeroPowerMultiplier is int factor)
                    multiplier = checked(multiplier * factor);
            }

            return checked(Math.Max(0, amount) * multiplier);
        }
    }
}
