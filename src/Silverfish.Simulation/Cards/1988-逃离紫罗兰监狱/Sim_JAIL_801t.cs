using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 熔炼黄金元素
// Molten Gold Elemental
// 战吼：造成4点伤害。
// Battlecry: Deal 4 damage.
internal static class Sim_JAIL_801t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
