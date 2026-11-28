using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暗影步
// Shadowstep
// 将一个友方随从移回你的手牌，它的法力值消耗减少 （2）点。
// Return a friendly minion to your hand. It_costs (2) less.
[CardSim("EX1_144")]
internal static class Sim_EX1_144
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_GBL_002e.Rule.ApplyWhenReturned()]
    };
}
