using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 信仰圣契
// Libram of Faith
// 召唤三个3/3并具有圣盾的德莱尼。如果本牌的法力值消耗为（0）点，使其获得突袭。
// Summon three 3/3 Draenei with Divine Shield. If this costs (0), give them Rush.
internal static class Sim_GDB_139
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
