using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 森林之灵
// Spirits of the Forest
// 抉择：召唤三只2/3并具有嘲讽的狼；或者召唤两只4/3并具有风怒的猎鹰。
// Choose One - Summon three 2/3 Wolves with Taunt; or Summon two 4/3 Falcons with Windfury.
internal static class Sim_EDR_233
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
