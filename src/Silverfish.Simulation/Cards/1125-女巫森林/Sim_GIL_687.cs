using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 通缉令
// WANTED!
// 对一个随从造成$3点伤害。如果该随从死亡，将一张幸运币置入你的手牌。
// Deal $3 damage to a minion. If it dies, add a Coin to your hand.
internal static class Sim_GIL_687
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
