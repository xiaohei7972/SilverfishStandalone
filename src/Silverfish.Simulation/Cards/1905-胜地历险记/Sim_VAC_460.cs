using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 把经理叫来！
// Oh, Manager!
// 造成$2点伤害。连击：获取一张幸运币。
// Deal $2 damage. Combo: Get a Coin.
internal static class Sim_VAC_460
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
