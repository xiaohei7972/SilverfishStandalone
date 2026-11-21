using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 裂解魔杖
// Wand of Disintegration
// 沉默并消灭所有敌方随从。
// Silence and destroy all enemy minions.
internal static class Sim_VAC_464t16
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
