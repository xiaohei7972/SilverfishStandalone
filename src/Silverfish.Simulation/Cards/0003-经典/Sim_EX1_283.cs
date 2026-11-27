using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 冰霜元素
// Frost Elemental
// 战吼： 冻结一个角色。
// Battlecry: Freeze a_character.
[CardSim("EX1_283")]
internal static class Sim_EX1_283
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new FreezeEffect(FreezeTarget.Selected)]
    };
}
