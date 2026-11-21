using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 飞翼冲击
// Wing Blast
// 对一个随从造成$4点伤害。如果在本回合中有一个随从死亡，该牌的法力值消耗为（1）点。
// Deal $4 damage to a minion. If a minion died this turn, this costs (1).
internal static class Sim_GIL_518
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
