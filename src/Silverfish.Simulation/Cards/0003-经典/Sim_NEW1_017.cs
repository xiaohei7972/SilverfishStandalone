using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 鱼人杀手蟹
// Hungry Crab
// 战吼：消灭一个鱼人，并获得+2/+2。
// Battlecry: Destroy a Murloc and gain +2/+2.
[CardSim("NEW1_017")]
internal static class Sim_NEW1_017
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.MURLOC), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyMinionEffect(), Sim_NEW1_017e.Rule.ApplyTo(StatTarget.Source)]
    };
}
