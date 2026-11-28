using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 激寒急流
// Sleet Storm
// 造成$2点伤害。随机对一个敌方随从造成$1点伤害。
// Deal $2 damage. _Deal $1 damage to a _random enemy minion.
internal static class Sim_CATA_485
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
