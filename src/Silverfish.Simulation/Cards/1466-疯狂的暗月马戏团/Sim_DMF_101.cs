using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 焰火元素
// Firework Elemental
// 战吼：对一个随从造成3点伤害。腐蚀：改为12点。
// Battlecry: Deal 3 damage to a minion. Corrupt: Deal 12 instead.
internal static class Sim_DMF_101
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
