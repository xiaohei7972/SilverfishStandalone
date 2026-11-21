using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 整备团队
// Ready the Fleet
// 使一个友方随从和具有相同类型的其他友方随从获得+1/+2。
// Give +1/+2 to a friendly minion and your other minions that share a type with it.
internal static class Sim_TLC_441
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
