namespace Silverfish.Simulation.Cards;

// 战利品贮藏者
// Loot Hoarder
// 亡语：抽一张牌。
// Deathrattle: Draw a card.
[CardSim("EX1_096")]
internal static class Sim_EX1_096
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new DrawEffect(1)]
    };
}
