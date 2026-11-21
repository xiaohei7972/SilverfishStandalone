namespace Silverfish.Simulation.Cards;

// 世界之树的嫩枝
// Twig of the World Tree
// 亡语：复原你的法力水晶。
// Deathrattle: Refresh your Mana Crystals.
[CardSim("LOOT_392")]
internal static class Sim_LOOT_392
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new ChangeManaEffect(ManaOperation.Refresh, int.MaxValue)]
    };
}
