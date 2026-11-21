using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 侦探服
// Detective's Clothes
// 使一个随从获得+4/+4和突袭。
// Give a minion +4/+4 and Rush.
internal static class Sim_JAIL_447t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
