using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 法术黑曜石
// Onyx Spellstone
// 随机消灭两个敌方随从。@
// Destroy up to 2 random enemy minions. @
internal static class Sim_LOOT_503t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
