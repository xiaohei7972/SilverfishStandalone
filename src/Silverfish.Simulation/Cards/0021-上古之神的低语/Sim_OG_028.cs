using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 深渊魔物
// Thing from Below
// 嘲讽 在本局对战中，你每召唤一个图腾，本牌的法力值消耗便减少（1）点。
// Taunt Costs (1) less for each Totem you've summoned this game.
[CardSim("OG_028")]
internal static class Sim_OG_028
{
    internal static CardRules Rules { get; } = new()
    {
        SummonedMinionTypeDiscount = MinionType.Totem,
    };
}
