using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 克苏恩之刃
// Blade of C'Thun
// 战吼：消灭一个随从。你的克苏恩会获得其攻击力和生命值（无论它在哪里）。
// Battlecry: Destroy a minion. Add its Attack and Health to_your C'Thun's (wherever it is).
[CardSim("OG_282")]
internal static class Sim_OG_282
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EmpowerCardEffect("OG_280", null, Sim_OG_282e.CardId) { FromSelectedStats = true },
        ],
    };
}
