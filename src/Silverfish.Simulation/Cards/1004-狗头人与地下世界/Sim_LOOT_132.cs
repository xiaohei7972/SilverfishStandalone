using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 屠龙者
// Dragonslayer
// 战吼：对一条龙造成6点伤害。
// Battlecry: Deal 6 damage to a Dragon.
internal static class Sim_LOOT_132
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DRAGON), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
