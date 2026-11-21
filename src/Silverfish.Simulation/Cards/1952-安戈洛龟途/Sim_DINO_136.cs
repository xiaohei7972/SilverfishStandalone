using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 盛宴之角
// Horn of Feasting
// 召唤三只2/1并具有突袭的迅猛龙。流放：使其在本回合中获得攻击时免疫。
// Summon three 2/1 Raptors with Rush. Outcast: Give them Immune while attacking this turn.
internal static class Sim_DINO_136
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
