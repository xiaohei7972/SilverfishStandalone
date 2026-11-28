namespace Silverfish.Simulation.Cards;

// 马尔考罗克
// Malkorok
// 战吼：随机装备一把武器。
// Battlecry: Equip a random weapon.
[CardSim("OG_220")]
internal static class Sim_OG_220
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EquipRandomWeaponEffect(),
        ],
    };
}
