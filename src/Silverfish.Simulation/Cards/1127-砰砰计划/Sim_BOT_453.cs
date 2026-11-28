using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迸射流星
// Shooting Star
// 对一个随从及其相邻的随从造成$1点伤害。
// Deal $1 damage to a minion and the minions next to it.
internal static class Sim_BOT_453
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
