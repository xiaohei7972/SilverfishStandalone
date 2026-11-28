using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 吸取灵魂
// Drain Soul
// 吸血 对一个随从造成 $3点伤害。
// Lifesteal Deal $3 damage to a minion.
internal static class Sim_CORE_ICC_055
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
