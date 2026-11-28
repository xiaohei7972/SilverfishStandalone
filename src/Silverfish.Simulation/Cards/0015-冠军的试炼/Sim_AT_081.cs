using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 纯洁者耶德瑞克
// Eadric the Pure
// 战吼：使所有敌方随从的攻击力变为1。
// Battlecry: Change all enemy minions' Attack to 1.
[CardSim("AT_081")]
internal static class Sim_AT_081
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_AT_081e.Rule.ApplyTo(StatTarget.EnemyMinions),
        ],
    };
}
