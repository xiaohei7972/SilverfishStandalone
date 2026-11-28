namespace Silverfish.Simulation.Cards;

// 暮光暗愈者
// Twilight Darkmender
// 战吼：如果你的克苏恩至少有10点攻击力，便为你的英雄恢复#10点生命值。
// Battlecry: If your C'Thun has at least 10 Attack, restore #10 Health to your hero.
[CardSim("OG_096")]
internal static class Sim_OG_096
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new PermanentCardAttackCondition("OG_280", 10), [new HealEffect(HealTarget.FriendlyHero, 10)]),
        ],
    };
}
