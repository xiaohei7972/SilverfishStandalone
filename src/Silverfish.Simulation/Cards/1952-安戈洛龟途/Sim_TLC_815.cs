using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 墓晨虚空芽
// Gravedawn Voidbulb
// 随机召唤一个法力值消耗为（4）的随从并使其获得嘲讽。延系：重复一次。
// Summon a random 4-Cost minion and give it Taunt. Kindred: Do it again.
internal static class Sim_TLC_815
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
