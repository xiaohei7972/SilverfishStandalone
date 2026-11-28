namespace Silverfish.Simulation.Cards;

// 提里奥·弗丁
// Tirion Fordring
// 圣盾。嘲讽。亡语：&装备一把5/3的灰烬使者。
// Divine Shield, Taunt Deathrattle:& Equip a 5/3_Ashbringer.
[CardSim("EX1_383")]
internal static class Sim_EX1_383
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new EquipWeaponEffect("EX1_383t")]
    };
}
