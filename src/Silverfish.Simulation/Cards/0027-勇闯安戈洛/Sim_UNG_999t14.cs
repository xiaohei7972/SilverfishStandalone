using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;
namespace Silverfish.Simulation.Cards;

// 火山之力
// Volcanic Might
// +1/+1
// +1/+1
[CardSim("UNG_999t14")]
internal static class Sim_UNG_999t14
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        IsPlayChoiceOption = true,
        OnPlay = [Sim_UNG_999t14e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
