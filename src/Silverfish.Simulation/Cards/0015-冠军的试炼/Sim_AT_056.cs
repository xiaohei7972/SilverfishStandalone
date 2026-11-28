using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 强风射击
// Powershot
// 对一个随从及其相邻的随从造成$2点伤害。
// Deal $2 damage to a minion and the minions next to it.
[CardSim("AT_056")]
internal static class Sim_AT_056
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SplashDamageEffect(2, 2),
        ],
    };
}
