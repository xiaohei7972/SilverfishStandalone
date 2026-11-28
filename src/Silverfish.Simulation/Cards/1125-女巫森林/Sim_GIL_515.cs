using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 捕鼠人
// Ratcatcher
// 突袭，战吼：消灭一个友方随从，并获得其攻击力和生命值。
// Rush Battlecry: Destroy a friendly minion and gain its Attack and Health.
internal static class Sim_GIL_515
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
