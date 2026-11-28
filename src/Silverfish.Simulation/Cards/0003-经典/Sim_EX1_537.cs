using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 爆炸射击
// Explosive Shot
// 对一个随从造成$5点伤害，并对其相邻的随从造成 $2点伤害。
// Deal $5 damage to a minion and $2 damage to adjacent ones.
[CardSim("EX1_537")]
internal static class Sim_EX1_537
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SplashDamageEffect(5, 2)]
    };
}
