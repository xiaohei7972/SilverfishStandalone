namespace Silverfish.Simulation;

/// <summary>独立定义手牌费用附魔，由施加效果的单卡 Sim 指定目标。</summary>
internal sealed record CostEnchantmentRule(string CardId, int Delta,
    EnchantmentDuration Duration = EnchantmentDuration.Permanent)
{
    internal ReturnMinionEffect ApplyWhenReturned(EffectTarget target = EffectTarget.Selected) =>
        new(Delta, target) { EnchantmentCardId = CardId, CostDuration = Duration };

    internal DrawEffect ApplyWhenDrawn(int count = 1) =>
        new(count, CostDelta: Delta, EnchantmentCardId: CardId);
}
