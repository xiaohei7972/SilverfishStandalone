using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 巨狼的恩赐
// Grace of the Greatwolf
// 抉择：对敌方英雄造成$4点伤害；或者召唤两只3/2并具有突袭的狼。
// Choose One - Deal $4 damage to the enemy hero; or Summon two 3/2 Wolves with Rush.
internal static class Sim_EDR_263
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
