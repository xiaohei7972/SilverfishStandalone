using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 惊爆异变
// Explosive Evolution
// 将一个随从随机变形成为一个法力值消耗增加（3）点的 随从。
// Transform a minion into a random one that costs (3) more.
internal static class Sim_YOD_020
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
