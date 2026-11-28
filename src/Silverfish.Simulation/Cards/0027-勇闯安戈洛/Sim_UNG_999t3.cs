using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;
namespace Silverfish.Simulation.Cards;

// 烈焰利爪
// Flaming Claws
// +3攻击力
// +3 Attack
[CardSim("UNG_999t3")]
internal static class Sim_UNG_999t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        IsPlayChoiceOption = true,
        OnPlay = [Sim_UNG_999t3e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
