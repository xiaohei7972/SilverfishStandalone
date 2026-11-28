using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 化学怪人
// Kooky Chemist
// 战吼： 使一个随从的攻击力和生命值互换。
// Battlecry: Swap the Attack and Health of a minion.
[CardSim("CFM_063")]
internal static class Sim_CFM_063
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SwapHealthEffect()],
    };
}
