using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 夜隐者圣所
// Nightcloak Sanctum
// 冻结一个随从。召唤一个2/2的不稳定的骷髅。
// Freeze a minion. Summon a 2/2 Volatile Skeleton.
internal static class Sim_REV_602
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
