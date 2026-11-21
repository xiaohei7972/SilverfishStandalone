using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 平静
// Pacified
// 将所有敌方随从的攻击力和生命值变为2。
// Set the Attack and Health of all enemy minions to 2.
internal static class Sim_TTN_858t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS)],
        []);
    // </legacy-play-requirements>

}
