using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 重碾
// Crush
// 消灭一个随从。如果你控制任何受伤的随从，该法术的法力值消耗减少（4）点。
// Destroy a minion. If you have a damaged minion, this costs (4) less.
[CardSim("GVG_052")]
internal static class Sim_GVG_052
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.FourIfControllingDamagedMinion,
        OnPlay = [new DestroyMinionEffect()]
    };
}
