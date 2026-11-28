using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 卡拉克西织珀者
// Klaxxi Amber-Weaver
// 嘲讽。战吼： 如果你的克苏恩至少有10点攻击力，便获得+5生命值。
// Taunt Battlecry: If your C'Thun has at least 10 Attack, gain +5 Health.
[CardSim("OG_188")]
internal static class Sim_OG_188
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new PermanentCardAttackCondition("OG_280", 10), [Sim_OG_188e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
