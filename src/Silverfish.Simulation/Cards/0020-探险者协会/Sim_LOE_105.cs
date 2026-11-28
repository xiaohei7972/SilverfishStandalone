using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 探险帽
// Explorer's Hat
// 使一个随从获得+1/+1，以及“亡语：获取一张探险帽”。
// Give a minion +1/+1 and "Deathrattle: Get an Explorer's Hat."
[CardSim("LOE_105")]
internal static class Sim_LOE_105
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
            Sim_LOE_105e.Rule.ApplyTo(StatTarget.Selected),
        ],
    };
}
