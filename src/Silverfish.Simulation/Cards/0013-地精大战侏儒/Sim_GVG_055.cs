using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 废旧螺栓机甲
// Screwjank Clunker
// 战吼：使一个友方机械获得+2/+2。
// Battlecry: Give a friendly Mech +2/+2.
[CardSim("GVG_055")]
internal static class Sim_GVG_055
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.MECHANICAL), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, 2, 2))
            {
                EnchantmentCardId = Sim_GVG_055e.CardId
            }
        ]
    };
}
