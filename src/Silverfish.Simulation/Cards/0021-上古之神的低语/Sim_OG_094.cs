using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 真言术：触
// Power Word: Tentacles
// 使一个随从获得+2/+6。
// Give a minion +2/+6.
[CardSim("OG_094")]
internal static class Sim_OG_094
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_OG_094e.Rule.ApplyTo(StatTarget.Selected),
        ],
    };
}
