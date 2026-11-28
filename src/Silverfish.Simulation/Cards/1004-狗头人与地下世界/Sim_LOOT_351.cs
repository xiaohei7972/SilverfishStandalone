namespace Silverfish.Simulation.Cards;

// 贪婪的林精
// Greedy Sprite
// 亡语：获得一个空的法力水晶。
// Deathrattle: Gain an empty Mana Crystal.
[CardSim("LOOT_351")]
internal static class Sim_LOOT_351
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new ChangeManaEffect(ManaOperation.GainEmpty, 1)]
    };
}
