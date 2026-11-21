using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黏液喷射者
// Gloop Sprayer
// 战吼： 为相邻的随从各召唤一个复制。
// Battlecry: Summon a copy of each adjacent minion.
internal static class Sim_BOT_507
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
