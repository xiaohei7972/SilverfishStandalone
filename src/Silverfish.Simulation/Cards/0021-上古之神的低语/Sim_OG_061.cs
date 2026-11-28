using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 搜寻猎物
// On the Hunt
// 造成$1点伤害。召唤一个1/1的獒犬。
// Deal $1 damage. Summon a 1/1 Mastiff.
[CardSim("OG_061")]
internal static class Sim_OG_061
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.Selected, 1),
            new SummonEffect("OG_061t"),
        ],
    };
}
