using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 洛丹伦城郊
// Outskirts of Lordaeron
// 选择一个随从，消灭所有攻击力小于它的随从。
// Choose a minion. Destroy all minions with less Attack than it.
internal static class Sim_WON_053t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
