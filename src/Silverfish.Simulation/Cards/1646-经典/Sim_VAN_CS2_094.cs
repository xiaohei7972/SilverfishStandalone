using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 愤怒之锤
// Hammer of Wrath
// 造成$3点伤害。抽一张牌。
// Deal $3 damage. Draw a card.
internal static class Sim_VAN_CS2_094
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
