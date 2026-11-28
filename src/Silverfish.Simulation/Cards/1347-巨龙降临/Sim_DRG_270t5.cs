using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 玛里苟斯的冰霜新星
// Malygos's Nova
// 冻结所有敌方随从。
// Freeze all enemy minions.
internal static class Sim_DRG_270t5
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
