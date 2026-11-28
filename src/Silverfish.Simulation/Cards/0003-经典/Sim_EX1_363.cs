using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 智慧祝福
// Blessing of Wisdom
// 选择一个随从，每当其进行攻击，便抽一张牌。
// Choose a minion. Whenever it attacks, draw a card.
[CardSim("EX1_363")]
internal static class Sim_EX1_363
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_363e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
