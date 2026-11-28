using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 能量之光
// Lantern of Power
// 使一个随从获得+10/+10。
// Give a minion +10/+10.
[CardSim("LOEA16_3")]
internal static class Sim_LOEA16_3
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_LOEA16_3e.Rule.ApplyTo(StatTarget.Selected),
        ],
    };
}
