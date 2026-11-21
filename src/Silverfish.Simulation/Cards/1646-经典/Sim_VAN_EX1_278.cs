using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 毒刃
// Shiv
// 造成$1点伤害。抽一张牌。
// Deal $1 damage. Draw a card.
internal static class Sim_VAN_EX1_278
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
