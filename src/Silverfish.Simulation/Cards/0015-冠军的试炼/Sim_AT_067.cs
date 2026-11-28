namespace Silverfish.Simulation.Cards;

// 猛犸人头领
// Magnataur Alpha
// 同时对其攻击目标相邻的随从造成伤害。
// Also damages the minions next to whomever he attacks.
[CardSim("AT_067")]
internal static class Sim_AT_067
{
    internal static CardRules Rules { get; } = new()
    {
        CleaveAdjacentOnAttack = true,
    };
}
