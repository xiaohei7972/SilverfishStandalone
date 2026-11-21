using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 苦修
// Penance
// 吸血 对一个随从造成$3点伤害。
// Lifesteal Deal $3 damage to a_minion.
internal static class Sim_ULD_714
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
