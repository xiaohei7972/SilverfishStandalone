namespace Silverfish.Simulation.Cards;

// 上古看守者
// Ancient Watcher
// 无法攻击。
// Can't attack.
[CardSim("EX1_045")]
internal static class Sim_EX1_045
{
    internal static CardRules Rules { get; } = new()
    {
        CannotAttack = true
    };
}
