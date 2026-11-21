using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虚空之刺
// Void Spike
// 造成$5点伤害。每回合翻转。
// Deal $5 damage. Flip each turn.
internal static class Sim_AV_207p2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
