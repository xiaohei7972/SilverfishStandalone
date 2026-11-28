using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 兽栏大师
// Stablemaster
// 战吼：在本回合中，使一个友方野兽获得免疫。
// Battlecry: Give a friendly Beast Immune this turn.
[CardSim("AT_057")]
internal static class Sim_AT_057
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_AT_057o.Rule.ApplyTo(StatTarget.Selected),
        ],
    };
}
