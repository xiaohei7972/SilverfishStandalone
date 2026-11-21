using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 海潮涌动
// Tidal Surge
// 吸血 对一个随从造成$5点伤害。
// Lifesteal Deal $5 damage to a_minion.
internal static class Sim_CORE_UNG_817
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
