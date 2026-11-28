using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 沃金
// Vol'jin
// 战吼：与另一个随从交换生命值。
// Battlecry: Swap Health with another minion.
[CardSim("GVG_014")]
internal static class Sim_GVG_014
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SwapHealthEffect()]
    };
}
