using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 鬼灵匪贼
// Spectral Pillager
// 连击：在本回合中，你每使用一张其他牌，便对一个随从造成2点伤害。
// Combo: Deal 2 damage to a minion for each other card you've played this turn.
internal static class Sim_ICC_910
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_FOR_COMBO)],
        []);
    // </legacy-play-requirements>

}
