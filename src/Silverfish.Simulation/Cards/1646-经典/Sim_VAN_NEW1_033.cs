using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雷欧克
// Leokk
// 你的其他随从拥有+1攻击力。
// Your other minions have +1 Attack.
internal static class Sim_VAN_NEW1_033
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
