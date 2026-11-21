using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灼燃之心
// Burning Heart
// 对一个随从造成$2点伤害，如果它依然存活，使你的英雄在本回合中获得+3攻击力。
// Deal $2 damage to a minion. If it survives, give your hero +3 Attack this turn.
internal static class Sim_DEEP_011
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
