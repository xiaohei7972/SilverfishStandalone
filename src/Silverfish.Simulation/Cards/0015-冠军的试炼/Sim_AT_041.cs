using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 荒野骑士
// Knight of the Wild
// 在本局对战中，你每召唤过一只野兽，本牌的法力值消耗便减少（1）点。
// Costs (1) less for each Beast you've summoned this game.
[CardSim("AT_041")]
internal static class Sim_AT_041
{
    internal static CardRules Rules { get; } = new()
    {
        SummonedMinionTypeDiscount = MinionType.Beast,
    };
}
