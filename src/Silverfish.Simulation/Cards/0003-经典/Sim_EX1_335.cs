namespace Silverfish.Simulation.Cards;

// 光耀之子
// Lightspawn
// 本随从的攻击力始终等同于其生命值。
// This minion's Attack is always equal to its Health.
[CardSim("EX1_335")]
internal static class Sim_EX1_335
{
    internal static CardRules Rules { get; } = new()
    {
        AttackEqualsHealth = true
    };
}
