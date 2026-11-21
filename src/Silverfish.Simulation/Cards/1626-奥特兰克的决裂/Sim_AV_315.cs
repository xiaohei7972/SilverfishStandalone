using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 超脱
// Deliverance
// 对一个随从造成$3点伤害。荣誉消灭：召唤该随从的一个3/3的新的复制。
// Deal $3 damage to a minion. Honorable Kill: Summon a new 3/3 copy of it.
internal static class Sim_AV_315
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
