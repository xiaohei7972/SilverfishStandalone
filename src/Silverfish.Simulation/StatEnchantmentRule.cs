using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

/// <summary>定义附魔的属性修正及按移除原因执行的额外效果。</summary>
internal sealed record StatEnchantmentRule(
    string CardId, StatModifier Modifier, EnchantmentDuration Duration = EnchantmentDuration.Permanent)
{
    internal ImmutableArray<EnchantmentRemovalRule> OnRemoved { get; init; }

    internal EnchantStatsEffect ApplyTo(StatTarget target) => new(target, Modifier, Duration)
    {
        EnchantmentCardId = CardId, OnRemoved = OnRemoved
    };
    internal StatAura GrantTo(AuraRecipients recipients) => new(recipients, Modifier) { EnchantmentCardId = CardId };
}
