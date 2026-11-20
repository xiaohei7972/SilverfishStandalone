using Silverfish.Domain;

namespace Silverfish.Simulation;

/// <summary>按卡牌 ID 定义可复用的附魔数值。施加方式由引用它的单卡规则指定。</summary>
internal sealed record StatEnchantmentRule(
    string CardId, StatModifier Modifier, EnchantmentDuration Duration = EnchantmentDuration.Permanent)
{
    internal EnchantStatsEffect ApplyTo(StatTarget target) => new(target, Modifier, Duration) { EnchantmentCardId = CardId };
    internal StatAura GrantTo(AuraRecipients recipients) => new(recipients, Modifier) { EnchantmentCardId = CardId };
}
