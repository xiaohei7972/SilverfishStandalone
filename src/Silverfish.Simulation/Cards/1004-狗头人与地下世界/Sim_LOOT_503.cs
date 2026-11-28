using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小型法术黑曜石
// Lesser Onyx Spellstone
// 随机消灭一个敌方随从。@（使用三张亡语牌后升级。）
// Destroy 1 random enemy minion. @(Play 3 Deathrattle cards to upgrade.)
internal static class Sim_LOOT_503
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
