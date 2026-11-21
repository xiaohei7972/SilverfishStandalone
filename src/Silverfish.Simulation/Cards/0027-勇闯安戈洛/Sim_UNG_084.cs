using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火羽凤凰
// Fire Plume Phoenix
// 战吼：造成3点伤害。
// Battlecry: Deal 3 damage.
internal static class Sim_UNG_084
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
