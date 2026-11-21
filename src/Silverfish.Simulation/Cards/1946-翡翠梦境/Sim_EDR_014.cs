using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 茏葱梦刃豹
// Verdant Dreamsaber
// 战吼：如果本牌的法力值消耗小于或等于（3）点，&随机攻击两个敌方随从。
// Battlecry: If this costs (3) or less,& attack two random enemy minions.
internal static class Sim_EDR_014
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
