using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 伪装大师
// Master of Disguise
// 战吼：直到你的下个回合，使一个友方随从获得潜行。
// Battlecry: Give a friendly minion Stealth until your next turn.
[CardSim("NEW1_014")]
internal static class Sim_NEW1_014
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_NEW1_014e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
