using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 光子炮台
// Photon Cannon
// 造成$3点伤害。 如果本牌消灭了一个随从，则在本局对战中，你的星灵随从牌的法力值消耗 减少（1）点。
// Deal $3 damage. If this kills a minion, your Protoss minions cost (1) less this game.
internal static class Sim_SC_753
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
