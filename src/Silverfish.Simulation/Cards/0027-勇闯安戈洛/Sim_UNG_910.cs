using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 凶残撕咬
// Grievous Bite
// 对一个随从造成$3点伤害，并对其相邻的随从 造成$1点伤害。
// Deal $3 damage to a minion and $1 damage to adjacent ones.
internal static class Sim_UNG_910
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
