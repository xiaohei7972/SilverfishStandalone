using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 迷乱
// Confuse
// 将所有随从的攻击力和生命值 互换。
// Swap the Attack and Health of all minions.
[CardSim("AT_016")]
internal static class Sim_AT_016
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DynamicStatChangeEffect(StatTarget.AllMinions, DynamicStatMode.SwapAttackAndHealth, Sim_AT_016e.CardId),
        ],
    };
}
