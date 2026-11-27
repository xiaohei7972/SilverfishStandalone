using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 银色保卫者
// Argent Protector
// 战吼：使一个其他友方随从获得圣盾。
// Battlecry: Give a friendly minion Divine Shield.
[CardSim("EX1_362")]
internal static class Sim_EX1_362
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EnchantStatsEffect(StatTarget.Selected, null) { GrantedTag = GAME_TAG.DIVINE_SHIELD }]
    };
}
