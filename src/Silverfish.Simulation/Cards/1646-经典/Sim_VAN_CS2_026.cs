using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰霜新星
// Frost Nova
// 冻结所有敌方随从。
// Freeze all enemy minions.
internal static class Sim_VAN_CS2_026
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
