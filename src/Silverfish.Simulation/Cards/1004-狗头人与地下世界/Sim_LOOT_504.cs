using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 不稳定的异变
// Unstable Evolution
// 回响 将一个友方随从随机变形成为一个法力值消耗增加（1）点的随从。
// Echo Transform a friendly minion into one that costs (1) more.
internal static class Sim_LOOT_504
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
