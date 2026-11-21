using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 侏儒变形师
// Recombobulator
// 战吼： 将一个友方随从随机变形成为一个法力值消耗相同的随从。
// Battlecry: Transform a friendly minion into a random minion with the same Cost.
internal static class Sim_GVG_108
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
