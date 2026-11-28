using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;
namespace Silverfish.Simulation.Cards;

// 岩质甲壳
// Rocky Carapace
// +3生命值
// +3 Health
[CardSim("UNG_999t4")]
internal static class Sim_UNG_999t4
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        IsPlayChoiceOption = true,
        OnPlay = [Sim_UNG_999t4e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
