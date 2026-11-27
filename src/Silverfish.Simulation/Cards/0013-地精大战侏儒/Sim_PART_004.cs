using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 隐秘力场
// Finicky Cloakfield
// 直到你的下个回合，使一个友方随从获得潜行。
// Give a friendly minion Stealth until your next turn.
[CardSim("PART_004")]
internal static class Sim_PART_004
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.Selected, null, EnchantmentDuration.UntilNextTurn)
            {
                GrantedTag = GAME_TAG.STEALTH,
                EnchantmentCardId = Sim_PART_004e.CardId
            }
        ]
    };
}
