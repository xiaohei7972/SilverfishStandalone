using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 诺甘农
// Norgannon
// 泰坦 在本随从使用一个技能后，其他技能的效果翻倍。
// Titan After this uses an ability, double the power of the other abilities.
internal static class Sim_TTN_075
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
