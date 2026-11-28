using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 病变虫群
// Morbid Swarm
// 抉择：召唤两只1/1的蚂蚁；或者消耗2份残骸，对一个随从造成$4点伤害。
// Choose One - Summon two 1/1 Ants; or Spend 2 Corpses to deal $4_damage to a minion.
internal static class Sim_EDR_813
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
