using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 持枪侍从
// Lance Carrier
// 战吼：使一个友方随从获得+2攻击力。
// Battlecry: Give a friendly minion +2 Attack.
[CardSim("AT_084")]
internal static class Sim_AT_084
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_AT_084e.Rule.ApplyTo(StatTarget.Selected),
        ],
    };
}
