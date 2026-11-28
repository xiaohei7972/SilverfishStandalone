using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 军情七处特工
// SI:7 Agent
// 连击：造成3点伤害。
// Combo: Deal 3 damage.
[CardSim("EX1_134")]
internal static class Sim_EX1_134
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_FOR_COMBO)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Selected, 3)]
    };
}
