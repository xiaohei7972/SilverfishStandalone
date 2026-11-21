namespace Silverfish.Simulation.Cards;

// 野性成长
// Wild Growth
// 获得一个空的法力水晶。
// Gain an empty Mana Crystal.
[CardSim("CS2_013")]
internal static class Sim_CS2_013
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 1, OverflowCardId: "CS2_013t")]
    };
}
