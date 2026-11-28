namespace Silverfish.Simulation.Cards;

// 死神4000型
// Foe Reaper 4000
// 同时对其攻击目标相邻的随从造成伤害。
// Also damages the minions next to whomever it attacks.
[CardSim("GVG_113")]
internal static class Sim_GVG_113
{
    internal static CardRules Rules { get; } = new()
    {
        CleaveAdjacentOnAttack = true
    };
}
