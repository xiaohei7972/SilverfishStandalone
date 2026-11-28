namespace Silverfish.Simulation.Cards;

// 破海者
// Sea Reaver
// 当你抽到该牌时，对你的随从造成 1点伤害。
// When you draw this, deal 1 damage to your minions.
[CardSim("AT_130")]
internal static class Sim_AT_130
{
    internal static CardRules Rules { get; } = new()
    {
        WhenDrawn =
        [
            new DamageEffect(DamageTarget.FriendlyMinions, 1),
        ],
    };
}
