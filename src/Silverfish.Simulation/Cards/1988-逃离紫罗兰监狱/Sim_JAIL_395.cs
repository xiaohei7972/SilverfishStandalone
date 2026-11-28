using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 下水道游水鳄
// Sewer Swimmer
// 预备。战吼：触发一个友方随从的亡语。
// Prepare Battlecry: Trigger a friendly _minion's Deathrattle.
internal static class Sim_JAIL_395
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
