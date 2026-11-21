using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 侧翼打击
// Flanking Strike
// 对一个随从造成$3点伤害。召唤一只3/3的狼。
// Deal $3 damage to a minion. Summon a 3/3 Wolf.
internal static class Sim_LOOT_077
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
