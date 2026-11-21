using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 焰火元素
// Firework Elemental
// 已腐蚀 战吼：对一个随从造成12点伤害。
// Corrupted Battlecry: Deal 12 damage to a minion.
internal static class Sim_DMF_101t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
