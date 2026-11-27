namespace Silverfish.Simulation.Cards;

// 阿拉希武器匠
// Arathi Weaponsmith
// 战吼：装备一把2/2的武器。
// Battlecry: Equip a 2/2_weapon.
[CardSim("EX1_398")]
internal static class Sim_EX1_398
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EquipWeaponEffect("EX1_398t")]
    };
}
