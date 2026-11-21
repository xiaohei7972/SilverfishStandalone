using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 喂食时间
// Feeding Time
// 对一个随从造成$3点伤害。召唤三只1/1的翼手龙并使其进化。
// Deal $3 damage to a minion. Summon three 1/1 Pterrordaxes and Adapt them.
internal static class Sim_UNG_834
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
