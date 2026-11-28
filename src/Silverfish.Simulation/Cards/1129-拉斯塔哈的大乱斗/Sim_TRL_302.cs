using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暂避锋芒
// Time Out!
// 直到你的 下个回合，你的英雄免疫。
// Your hero is Immune until your next turn.
internal static class Sim_TRL_302
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
