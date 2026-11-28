using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 绿皮船长
// Captain Greenskin
// 战吼：使你的武器获得+1/+1。
// Battlecry: Give your weapon +1/+1.
[CardSim("NEW1_024")]
internal static class Sim_NEW1_024
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_NEW1_024o.Rule.ApplyToWeapon()]
    };
}
