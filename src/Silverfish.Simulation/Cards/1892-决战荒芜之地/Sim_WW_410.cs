using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 三条七
// Triple Sevens
// 对一个随从造成$7点伤害。 抽7张牌。
// Deal $7 damage to a minion. Draw 7 cards.
internal static class Sim_WW_410
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
