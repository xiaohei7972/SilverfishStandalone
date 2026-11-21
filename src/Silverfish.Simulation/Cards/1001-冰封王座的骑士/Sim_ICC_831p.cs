using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 生命虹吸
// Siphon Life
// 吸血，造成$3点伤害。
// Lifesteal Deal $3 damage.
internal static class Sim_ICC_831p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
