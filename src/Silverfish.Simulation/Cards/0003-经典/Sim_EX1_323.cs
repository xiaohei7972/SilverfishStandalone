namespace Silverfish.Simulation.Cards;

// 加拉克苏斯大王
// Lord Jaraxxus
// 战吼：&装备一把3/8的血怒。~GAMEPLAY_ASPROXY_EX1_323
// Battlecry:& Equip a 3/8 Blood Fury.~GAMEPLAY_ASPROXY_EX1_323
[CardSim("EX1_323")]
internal static class Sim_EX1_323
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new GainArmorEffect(5),
            new ReplaceHeroPowerEffect("EX1_tk33"),
            new EquipWeaponEffect("EX1_323w")
        ]
    };
}
