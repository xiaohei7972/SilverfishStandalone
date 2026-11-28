namespace Silverfish.Simulation.Cards;

// 盗墓匪贼
// Tomb Pillager
// 亡语：获取一张 幸运币。
// Deathrattle: Get a Coin.
[CardSim("LOE_012")]
internal static class Sim_LOE_012
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new CreateCardEffect("GAME_005", Count: 1),
        ],
    };
}
