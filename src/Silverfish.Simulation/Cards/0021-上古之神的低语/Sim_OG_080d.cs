using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 石南草毒素
// Briarthorn Toxin
// 使一个随从获得+3攻击力。
// Give a minion +3 Attack.
[CardSim("OG_080d")]
internal static class Sim_OG_080d
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
            Sim_OG_080ee.Rule.ApplyTo(StatTarget.Selected),
        ],
    };
}
