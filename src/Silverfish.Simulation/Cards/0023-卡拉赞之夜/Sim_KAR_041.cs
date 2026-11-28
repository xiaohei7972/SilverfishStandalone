using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 沟渠潜伏者
// Moat Lurker
// 战吼：消灭一个随从。亡语：再次召唤被消灭的随从。
// Battlecry: Destroy a minion. Deathrattle: Resummon it.
[CardSim("KAR_041")]
internal static class Sim_KAR_041
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyAndCaptureMinionEffect(Sim_KAR_041e.Rule.ApplyTo(StatTarget.Source))],
    };
}
