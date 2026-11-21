using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 军情七处特工
// SI:7 Agent
// 连击：造成2点伤害。
// Combo: Deal 2 damage.
internal static class Sim_VAN_EX1_134
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_FOR_COMBO)],
        []);
    // </legacy-play-requirements>

}
