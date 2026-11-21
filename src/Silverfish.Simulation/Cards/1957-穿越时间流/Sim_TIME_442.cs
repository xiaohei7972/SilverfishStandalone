using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 时间流守望者
// Timeway Warden
// 战吼：囚禁一个敌方随从，使其休眠10000回合。亡语：唤醒该随从。
// Battlecry: Imprison an enemy minion. It goes Dormant for 10,000 turns. ___Deathrattle: Awaken it.
internal static class Sim_TIME_442
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
