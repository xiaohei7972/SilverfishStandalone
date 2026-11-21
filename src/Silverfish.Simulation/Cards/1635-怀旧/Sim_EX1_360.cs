using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 谦逊
// Humility
// 使一个随从的攻击力变为1。
// Change a minion's Attack to 1.
[CardSim("EX1_360")]
internal static class Sim_EX1_360
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_360e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
