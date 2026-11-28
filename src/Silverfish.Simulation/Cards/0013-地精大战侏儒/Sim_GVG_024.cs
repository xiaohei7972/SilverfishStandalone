namespace Silverfish.Simulation.Cards;

// 齿轮大师的扳手
// Cogmaster's Wrench
// 如果你控制任何机械，便拥有 +2攻击力。
// Has +2 Attack while you have a Mech.
[CardSim("GVG_024")]
internal static class Sim_GVG_024
{
    internal static CardRules Rules { get; } = new()
    {
        WeaponAuras = [new WeaponAttackAura(2, AuraCondition.WhileControllingMech)]
    };
}
