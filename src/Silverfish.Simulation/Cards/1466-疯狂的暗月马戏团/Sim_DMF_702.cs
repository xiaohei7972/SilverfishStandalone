using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 风暴打击
// Stormstrike
// 对一个随从造成$3点伤害。在本回合中，使你的英雄获得+3攻击力。
// Deal $3 damage to a minion. Give your hero +3 Attack this turn.
internal static class Sim_DMF_702
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
