using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 圣殿执行者
// Temple Enforcer
// 战吼：使一个友方随从获得+3生命值。
// Battlecry: Give a friendly minion +3 Health.
[CardSim("EX1_623")]
internal static class Sim_EX1_623
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_623e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
