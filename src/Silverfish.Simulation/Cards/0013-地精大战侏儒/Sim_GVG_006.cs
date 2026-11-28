using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 机械跃迁者
// Mechwarper
// 你的机械的法力值消耗减少（1）点。
// Your Mechs cost (1) less.
[CardSim("GVG_006")]
internal static class Sim_GVG_006
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras =
        [
            new CostAura(CostAuraRecipient.Friendly, CostAuraTarget.Minion, -1)
            {
                CardFilter = new CardFilter { MinionType = MinionType.Mech }
            }
        ]
    };
}
