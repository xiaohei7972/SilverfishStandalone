using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 麻痹睡眠
// Sleep Paralysis
// 抉择：召唤两个3/6并具有嘲讽且无法攻击的恶魔；或者消灭一个敌方随从。
// Choose One - Summon two 3/6 Demons with Taunt that can't attack; or Destroy an enemy minion.
internal static class Sim_EDR_490
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
