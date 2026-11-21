using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 亡者复生
// Raise Dead
// 对你的英雄造成$3点伤害。将两个在本局对战中死亡的友方随从移回你的手牌。
// Deal $3 damage to your hero. Return two friendly minions that died this game to your hand.
internal static class Sim_SCH_514
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_GAME)],
        []);
    // </legacy-play-requirements>

}
