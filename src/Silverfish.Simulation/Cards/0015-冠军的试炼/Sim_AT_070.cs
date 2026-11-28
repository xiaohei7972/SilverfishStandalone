namespace Silverfish.Simulation.Cards;

// 天空上尉库拉格
// Skycap'n Kragg
// 冲冲冲冲锋 每有一个友方海盗，本牌的法力值消耗便减少（1）点。
// Charrrrrge Costs (1) less for each friendly Pirate.
[CardSim("AT_070")]
internal static class Sim_AT_070
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.FriendlyPirates,
    };
}
