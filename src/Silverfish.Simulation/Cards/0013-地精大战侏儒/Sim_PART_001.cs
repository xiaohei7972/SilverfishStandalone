using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 重型护甲
// Armor Plating
// 使一个随从获得+1生命值。
// Give a minion +1 Health.
[CardSim("PART_001")]
internal static class Sim_PART_001
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, MaximumHealth: 1))
            {
                EnchantmentCardId = Sim_PART_001e.CardId
            }
        ]
    };
}
