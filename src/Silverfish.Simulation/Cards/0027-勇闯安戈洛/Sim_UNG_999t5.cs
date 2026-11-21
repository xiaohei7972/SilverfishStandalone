using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;
namespace Silverfish.Simulation.Cards;

// 液态外膜
// Liquid Membrane
// 扰魔
// Elusive
[CardSim("UNG_999t5")]
internal static class Sim_UNG_999t5
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        IsPlayChoiceOption = true,
        OnPlay = [Sim_UNG_999t5e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
