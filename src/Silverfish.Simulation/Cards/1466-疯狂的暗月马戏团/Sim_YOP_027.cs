using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 套索射击
// Bola Shot
// 对一个随从造成$1点伤害，并对其相邻的随从造成 $2点伤害。
// Deal $1 damage to a minion and $2 damage to its neighbors.
internal static class Sim_YOP_027
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
