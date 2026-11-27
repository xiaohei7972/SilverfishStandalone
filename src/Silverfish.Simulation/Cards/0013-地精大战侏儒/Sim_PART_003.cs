using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 生锈的号角
// Rusty Horn
// 使一个随从获得嘲讽。
// Give a minion Taunt.
[CardSim("PART_003")]
internal static class Sim_PART_003
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EnchantStatsEffect(StatTarget.Selected, null) { GrantedTag = GAME_TAG.TAUNT }]
    };
}
