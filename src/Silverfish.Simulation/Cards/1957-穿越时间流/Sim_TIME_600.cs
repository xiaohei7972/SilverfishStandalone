using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 精确射击
// Precise Shot
// 造成$3点伤害。如果本牌位于你手牌的正中间，改为造成 $5点。
// Deal $3 damage. If this is EXACTLY in the center of your hand, deal $5 instead.
internal static class Sim_TIME_600
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
