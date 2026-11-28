using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 狂暴
// Rampage
// 使一个受伤的随从获得+3/+3。
// Give a damaged minion +3/+3.
[CardSim("CS2_104")]
internal static class Sim_CS2_104
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_DAMAGED_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_CS2_104e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
