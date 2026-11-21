using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;
namespace Silverfish.Simulation.Cards;

// 氤氲迷雾
// Shrouding Mist
// 获得潜行直到你的下个回合。
// Stealth until your next turn.
[CardSim("UNG_999t10")]
internal static class Sim_UNG_999t10
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        IsPlayChoiceOption = true,
        OnPlay = [Sim_UNG_999t10e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
