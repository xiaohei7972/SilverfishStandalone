namespace Silverfish.Simulation.Cards;

// 末日降临
// DOOM!
// 消灭所有随从。每消灭一个随从，便抽一张牌。
// Destroy all minions. Draw a card for each.
[CardSim("OG_239")]
internal static class Sim_OG_239
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DestroyMinionsAndDrawEffect(),
        ],
    };
}
