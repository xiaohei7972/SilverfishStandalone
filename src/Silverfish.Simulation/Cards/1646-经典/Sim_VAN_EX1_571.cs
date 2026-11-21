using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 自然之力
// Force of Nature
// 召唤三个2/2并具有冲锋的树人，在回合结束时，消灭这些树人。
// Summon three 2/2 Treants with Charge that die at the end of the turn.
internal static class Sim_VAN_EX1_571
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
