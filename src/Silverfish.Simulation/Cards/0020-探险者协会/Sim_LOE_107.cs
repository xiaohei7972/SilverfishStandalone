namespace Silverfish.Simulation.Cards;

// 诡异的雕像
// Eerie Statue
// 除非它是战场上唯一的一个随从，否则无法进行攻击。
// Can’t attack unless it’s the only minion on the battlefield.
[CardSim("LOE_107")]
internal static class Sim_LOE_107
{
    internal static CardRules Rules { get; } = new()
    {
        AttacksOnlyIfLoneMinion = true,
    };
}
