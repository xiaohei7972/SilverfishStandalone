using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 近轨血月
// Orbital Moon
// 使一个随从获得 嘲讽和吸血。如果你在本回合中使用过相邻的牌，还会使其获得复生。
// Give a minion Taunt and Lifesteal. If you played an adjacent card this turn, also give it Reborn.
internal static class Sim_GDB_475
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
