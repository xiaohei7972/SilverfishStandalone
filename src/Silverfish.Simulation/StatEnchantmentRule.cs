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

    internal EnchantStatsEffect ApplyTo(StatTarget target) => new(target, Modifier, Duration)
    {
        EnchantmentCardId = CardId, OnRemoved = OnRemoved, GrantedTag = GrantedTag
    };
    internal StatAura GrantTo(AuraRecipients recipients) =>
        new(recipients, Modifier ?? throw new InvalidOperationException("属性光环必须包含身材修正。")) { EnchantmentCardId = CardId };
}
