namespace Silverfish.Simulation.Cards;

// 恩佐斯的副官
// N'Zoth's First Mate
// 战吼：装备一把1/3的锈蚀铁钩。
// Battlecry: Equip a 1/3 Rusty Hook.
[CardSim("OG_312")]
internal static class Sim_OG_312
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EquipWeaponEffect("OG_058"),
        ],
    };
}
