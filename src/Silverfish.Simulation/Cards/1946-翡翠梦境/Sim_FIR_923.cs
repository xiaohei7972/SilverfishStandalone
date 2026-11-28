using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 炎魔之火
// Flames of the Firelord
// 随机对一个 敌方随从造成$4点伤害。如果你的手牌中有法力值消耗大于或等于（8）点的牌，改为造成$8点。
// Deal $4 damage to a random enemy minion. If you're holding a card that costs (8) or more, deal $8 instead.
internal static class Sim_FIR_923
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
