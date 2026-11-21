using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 女巫跟班
// Witchy Lackey
// 战吼：将一个友方随从变形成为一个法力值消耗增加（1）点的随从。
// Battlecry: Transform a friendly minion into one that costs (1) more.
[CardSim("DAL_615")]
internal static class Sim_DAL_615
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new TransformMinionByCostEffect(1)]
    };
}
