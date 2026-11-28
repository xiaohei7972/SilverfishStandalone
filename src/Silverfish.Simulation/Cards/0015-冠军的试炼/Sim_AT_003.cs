namespace Silverfish.Simulation.Cards;

// 英雄之魂
// Fallen Hero
// 你的英雄技能会额外造成1点伤害。
// Your Hero Power deals 1_extra damage.
[CardSim("AT_003")]
internal static class Sim_AT_003
{
    internal static CardRules Rules { get; } = new()
    {
        FriendlyHeroPowerDamage = 1,
    };
}
