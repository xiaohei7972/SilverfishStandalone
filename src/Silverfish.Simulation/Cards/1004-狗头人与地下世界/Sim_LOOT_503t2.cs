using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大型法术黑曜石
// Greater Onyx Spellstone
// 随机消灭三个敌方随从。
// Destroy up to 3 random enemy minions.
internal static class Sim_LOOT_503t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
