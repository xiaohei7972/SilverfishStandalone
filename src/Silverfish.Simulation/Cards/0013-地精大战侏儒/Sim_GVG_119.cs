namespace Silverfish.Simulation.Cards;

// 布林顿3000型
// Blingtron 3000
// 战吼：为每个玩家装备一把武器。
// Battlecry: Equip a random weapon for each player.
[CardSim("GVG_119")]
internal static class Sim_GVG_119
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EquipRandomWeaponEachPlayerEffect()]
    };
}
