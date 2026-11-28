using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灵魂之火
// Soulfire
// 造成$4点伤害，随机弃一 张牌。
// Deal $4 damage. Discard a random card.
internal static class Sim_EX1_308
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
