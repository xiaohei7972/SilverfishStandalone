namespace Silverfish.Simulation.Cards;

// 雷霆绽放
// Lightning Bloom
// 复原两个法力水晶。过载：（2）
// Refresh 2 Mana Crystals. Overload: (2)
[CardSim("SCH_427")]
internal static class Sim_SCH_427
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.Refresh, 2)]
    };
}
