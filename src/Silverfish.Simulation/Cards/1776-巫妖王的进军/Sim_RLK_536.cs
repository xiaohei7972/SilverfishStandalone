using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 薄葬
// Shallow Grave
// 触发一个友方随从的亡语，然后将其消灭。
// Trigger a friendly minion's Deathrattle, then destroy it.
internal static class Sim_RLK_536
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE)],
        []);
    // </legacy-play-requirements>

}
