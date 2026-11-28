using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 玛维·影歌
// Maiev Shadowsong
// 战吼： 选择一个随从。使其休眠2回合。
// Battlecry: Choose a minion. It goes Dormant for 2 turns.
internal static class Sim_BT_737
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
