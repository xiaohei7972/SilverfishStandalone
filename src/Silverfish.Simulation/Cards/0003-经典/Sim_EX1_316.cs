using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 力量的代价
// Power Overwhelming
// 使一个友方随从获得+4/+4，该随从会在回合结束时死亡。死得很惨。
// Give a friendly minion +4/+4 until end of turn. Then, it dies. Horribly.
[CardSim("EX1_316")]
internal static class Sim_EX1_316
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_316e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
