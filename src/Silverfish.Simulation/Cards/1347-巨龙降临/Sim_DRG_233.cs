using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 沙尘吐息
// Sand Breath
// 使一个随从获得+1/+2。如果你的手牌中有龙牌，使其获得圣盾。
// Give a minion +1/+2. Give it Divine Shield if you're holding a Dragon.
internal static class Sim_DRG_233
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
