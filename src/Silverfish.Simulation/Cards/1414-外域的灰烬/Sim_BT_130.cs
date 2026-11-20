namespace Silverfish.Simulation.Cards;

// 过度生长
// Overgrowth
// 获得两个空的法力水晶。
// Gain two empty Mana_Crystals.
[CardSim("BT_130")]
internal static class Sim_BT_130
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 2, OverflowCardId: "CS2_013t")]
    };
}
