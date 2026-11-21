using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰龙吐息
// Breath of Sindragosa
// 随机对一个敌方随从造成$2点伤害，并使其冻结。
// Deal $2 damage to a random enemy minion and Freeze it.
internal static class Sim_ICC_836
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
