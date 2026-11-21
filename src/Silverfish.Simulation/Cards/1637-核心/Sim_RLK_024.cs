using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灵界打击
// Death Strike
// 吸血 对一个随从造成$6点伤害。
// Lifesteal Deal $6 damage to a minion.
internal static class Sim_RLK_024
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
