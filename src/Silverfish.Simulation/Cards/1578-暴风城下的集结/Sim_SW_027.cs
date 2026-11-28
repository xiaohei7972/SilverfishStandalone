using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 海上威胁
// Shiver Their Timbers!
// 对一个随从造成$2点伤害。如果你控制一个海盗，则改为造成$5点伤害。
// Deal $2 damage to a minion. If you control a Pirate, deal $5 instead.
internal static class Sim_SW_027
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
