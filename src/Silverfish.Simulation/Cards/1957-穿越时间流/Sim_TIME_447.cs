using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 真言术：障
// Power Word: Barrier
// 使一个角色获得圣盾。使你手牌中的随从牌获得+2生命值。
// Give a character Divine Shield. Give minions in your hand +2 Health.
internal static class Sim_TIME_447
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
