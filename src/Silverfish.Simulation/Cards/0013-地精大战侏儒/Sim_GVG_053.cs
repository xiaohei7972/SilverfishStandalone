namespace Silverfish.Simulation.Cards;

// 盾甲侍女
// Shieldmaiden
// 战吼： 获得5点护甲值。
// Battlecry: Gain 5 Armor.
[CardSim("GVG_053")]
internal static class Sim_GVG_053
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new GainArmorEffect(5)]
    };
}
