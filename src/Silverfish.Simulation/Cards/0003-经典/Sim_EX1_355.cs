using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 受祝福的勇士
// Blessed Champion
// 使一个随从的攻击力翻倍。
// Double a minion's Attack.
[CardSim("EX1_355")]
internal static class Sim_EX1_355
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DynamicStatChangeEffect(StatTarget.Selected, DynamicStatMode.DoubleAttack, Sim_EX1_355e.CardId)]
    };
}
