using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影步
// Shadowstep
// 将一个友方随从移回你的手牌，它的法力值消耗减少 （2）点。
// Return a friendly minion to your hand. It_costs (2) less.
internal static class Sim_CORE_EX1_144
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
