using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 奥达曼守护者
// Keeper of Uldaman
// 战吼： 将一个随从的攻击力和生命值变为3。
// Battlecry: Set a minion's Attack and Health to 3.
[CardSim("LOE_017")]
internal static class Sim_LOE_017
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
            Sim_LOE_017e.Rule.ApplyTo(StatTarget.Selected),
        ],
    };
}
