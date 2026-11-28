using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 荒野行者
// Wildwalker
// 战吼：使一个友方野兽获得+3生命值。
// Battlecry: Give a friendly Beast +3 Health.
[CardSim("AT_040")]
internal static class Sim_AT_040
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_AT_040e.Rule.ApplyTo(StatTarget.Selected),
        ],
    };
}
