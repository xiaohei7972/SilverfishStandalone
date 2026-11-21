using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 星火术
// Starfire
// 造成$5点伤害。抽一张牌。
// Deal $5 damage. Draw a card.
internal static class Sim_EX1_173
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
