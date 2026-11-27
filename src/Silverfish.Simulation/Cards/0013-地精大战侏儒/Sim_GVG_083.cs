using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 高级修理机器人
// Upgraded Repair Bot
// 战吼：使一个友方机械获得+4生命值。
// Battlecry: Give a friendly Mech +4 Health.
[CardSim("GVG_083")]
internal static class Sim_GVG_083
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.MECHANICAL), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, MaximumHealth: 4))
            {
                EnchantmentCardId = Sim_GVG_069a.CardId
            }
        ]
    };
}
