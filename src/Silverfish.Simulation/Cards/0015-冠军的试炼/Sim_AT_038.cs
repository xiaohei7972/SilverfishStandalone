namespace Silverfish.Simulation.Cards;

// 达纳苏斯豹骑士
// Darnassus Aspirant
// 战吼：获得一个空的法力水晶。 亡语：失去一个法力水晶。
// Battlecry: Gain an empty Mana Crystal. Deathrattle: Lose a Mana Crystal.
[CardSim("AT_038")]
internal static class Sim_AT_038
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 1)],
        Deathrattle = [new ChangeManaEffect(ManaOperation.Destroy, 1)]
    };
}
