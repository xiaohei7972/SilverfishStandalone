using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奖品掠夺者
// Prize Plunderer
// 连击：在本回合中，你每使用一张其他牌，便对一个随从造成1点伤害。
// Combo: Deal 1 damage to a minion for each other card you've played this turn.
internal static class Sim_DMF_519
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
