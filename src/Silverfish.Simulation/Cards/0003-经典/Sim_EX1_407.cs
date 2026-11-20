namespace Silverfish.Simulation.Cards;

// 绝命乱斗
// Brawl
// 随机选择一个随从，消灭除了该随从外的所有其他随从。
// Destroy all minions except one. (chosen randomly)
[CardSim("EX1_407")]
internal static class Sim_EX1_407
{
    internal static CardRules Rules { get; } = new()
    {
        MinimumTotalMinions = 2, OnPlay = [new DestroyAllButOneMinionEffect()]
    };
}
