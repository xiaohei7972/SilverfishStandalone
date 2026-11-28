using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 潜行者的伎俩
// Rogues Do It...
// 造成$4点伤害。抽一张牌。
// Deal $4 damage. Draw a card.
internal static class Sim_PRO_001b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
