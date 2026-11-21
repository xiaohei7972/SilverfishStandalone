using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 淤泥之池
// Muck Pools
// 将一个友方随从变形成为法力值消耗增加（1）点的随从。
// Transform a friendly minion into one that costs (1) more.
[CardSim("REV_923")]
internal static class Sim_REV_923
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        Activation = new([new TransformMinionByCostEffect(1)])
    };
}
