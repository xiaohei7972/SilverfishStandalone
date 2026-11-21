using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 降落伞
// Parachute
// 抽到时施放 召唤一个1/1并具有冲锋的海盗。
// Casts When Drawn Summon a 1/1 Pirate with Charge.
internal static class Sim_VAC_933t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
