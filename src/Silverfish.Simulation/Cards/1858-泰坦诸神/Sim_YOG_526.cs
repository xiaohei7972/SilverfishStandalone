using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 触须缠握
// Tentacle Grip
// 造成$3点伤害。连击：获取一张1/1的混乱触须。
// Deal $3 damage. Combo: Get a 1/1 Chaotic Tendril.
internal static class Sim_YOG_526
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
