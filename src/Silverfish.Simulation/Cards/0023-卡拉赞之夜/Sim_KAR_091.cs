namespace Silverfish.Simulation.Cards;

// 铁炉堡传送门
// Ironforge Portal
// 获得4点护甲值。随机召唤一个法力值消耗为（4）的 随从。
// Gain 4 Armor. Summon a random 4-Cost minion.
[CardSim("KAR_091")]
internal static class Sim_KAR_091
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new GainArmorEffect(4), new SummonMatchingMinionEffect(new CardFilter { MinimumPrintedCost = 4, MaximumPrintedCost = 4 })],
    };
}
