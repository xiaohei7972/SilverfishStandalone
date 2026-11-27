using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 疯狂的炼金师
// Crazed Alchemist
// 战吼： 使一个随从的攻击力和生命值互换。
// Battlecry: Swap the Attack and Health of a minion.
[CardSim("EX1_059")]
internal static class Sim_EX1_059
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DynamicStatChangeEffect(StatTarget.Selected, DynamicStatMode.SwapAttackAndHealth, Sim_EX1_059e.CardId)]
    };
}
