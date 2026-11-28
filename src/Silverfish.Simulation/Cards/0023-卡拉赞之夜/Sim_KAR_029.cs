namespace Silverfish.Simulation.Cards;

// 符文蛋
// Runic Egg
// 亡语：抽一张牌。
// Deathrattle: Draw a card.
[CardSim("KAR_029")]
internal static class Sim_KAR_029
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new DrawEffect(1)],
    };
}
