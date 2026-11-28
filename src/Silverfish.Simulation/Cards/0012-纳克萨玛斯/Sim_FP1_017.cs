using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 尼鲁巴蛛网领主
// Nerub'ar Weblord
// 具有战吼的随从法力值消耗增加（2）点。
// Minions with Battlecry cost (2) more.
[CardSim("FP1_017")]
internal static class Sim_FP1_017
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras =
        [
            new CostAura(
                CostAuraRecipient.BothPlayers,
                CostAuraTarget.Minion,
                2)
            {
                CardFilter = new()
                {
                    Kind = EntityKind.Minion,
                    HasBattlecry = true
                }
            }
        ]
    };
}
