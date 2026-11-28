using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 喷吐火焰
// Fire Breath
// 造成$4点伤害。使你的元素获得+1/+1。
// Deal $4 damage. Give your Elementals +1/+1.
internal static class Sim_DINO_406
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
