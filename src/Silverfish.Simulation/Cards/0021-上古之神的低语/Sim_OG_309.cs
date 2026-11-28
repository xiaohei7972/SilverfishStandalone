using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 哈霍兰公主
// Princess Huhuran
// 战吼：触发一个友方随从的亡语。
// Battlecry: Trigger a friendly minion's Deathrattle.
[CardSim("OG_309")]
internal static class Sim_OG_309
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new RecastDeathrattleEffect(),
        ],
    };
}
