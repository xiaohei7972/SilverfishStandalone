using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小鬼狱火
// Impferno
// 使你的恶魔获得+1攻击力。对所有敌方随从造成$1点 伤害。
// Give your Demons +1 Attack. Deal $1 damage to all enemy minions.
internal static class Sim_DAL_605
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DEMON), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
