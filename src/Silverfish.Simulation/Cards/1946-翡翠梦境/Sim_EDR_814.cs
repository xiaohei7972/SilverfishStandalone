using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 感染吐息
// Infested Breath
// 造成$2点伤害。召唤一条0/2的水蛭。
// Deal $2 damage. Summon a 0/2 Leech.
internal static class Sim_EDR_814
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
