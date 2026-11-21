using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;
namespace Silverfish.Simulation.Cards;

// 活性孢子
// Living Spores
// 亡语：召唤两个1/1的植物。
// Deathrattle: Summon two 1/1 Plants.
[CardSim("UNG_999t2")]
internal static class Sim_UNG_999t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        IsPlayChoiceOption = true,
        OnPlay = [Sim_UNG_999t2e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
