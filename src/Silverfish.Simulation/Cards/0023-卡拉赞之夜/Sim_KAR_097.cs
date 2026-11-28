namespace Silverfish.Simulation.Cards;

// 守护者麦迪文
// Medivh, the Guardian
// 战吼： 装备埃提耶什，守护者的传说之杖。
// Battlecry: Equip Atiesh, Greatstaff of the Guardian.
[CardSim("KAR_097")]
internal static class Sim_KAR_097
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EquipWeaponEffect("KAR_097t")],
    };
}
