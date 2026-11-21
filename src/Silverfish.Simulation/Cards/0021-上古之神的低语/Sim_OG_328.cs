using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 异变之主
// Master of Evolution
// 战吼：将一个友方随从随机变形成为一个法力值消耗增加（1）点的随从。
// Battlecry: Transform a friendly minion into a random one that costs (1) more.
internal static class Sim_OG_328
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
