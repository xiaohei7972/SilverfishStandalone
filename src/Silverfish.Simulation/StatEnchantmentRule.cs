using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

/// <summary>定义附魔的属性修正及按移除原因执行的额外效果。</summary>
internal sealed record StatEnchantmentRule(
    string CardId, StatModifier? Modifier, EnchantmentDuration Duration = EnchantmentDuration.Permanent)
{
    internal ImmutableArray<EnchantmentRemovalRule> OnRemoved { get; init; }
    internal GAME_TAG? GrantedTag { get; init; }

    internal EnchantStatsEffect ApplyTo(StatTarget target, CardFilter? cardFilter = null) => new(target, Modifier, Duration)
    {
        EnchantmentCardId = CardId, OnRemoved = OnRemoved, GrantedTag = GrantedTag, CardFilter = cardFilter
    };
    internal StatAura GrantTo(AuraRecipients recipients, AuraCondition condition = AuraCondition.None) =>
        new(recipients, Modifier)
        {
            EnchantmentCardId = CardId, Condition = condition, GrantedTag = GrantedTag
        };

    internal WeaponAttackAura GrantToWeapon(AuraCondition condition = AuraCondition.None) =>
        new(Modifier?.Attack ?? throw new InvalidOperationException("武器攻击光环必须包含攻击力修正。"), condition)
        {
            EnchantmentCardId = CardId
        };

    internal ModifyWeaponEffect ApplyToWeapon(ConditionPlayer target = ConditionPlayer.Friendly) =>
        new(Modifier?.Attack ?? 0, Modifier?.MaximumHealth ?? 0, target, CardId);
}
