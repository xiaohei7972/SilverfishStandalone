using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 圣水
// Holy Water
// 对一个随从造成$4点伤害。如果该随从死亡，将一张该随从的复制置入你的手牌。
// Deal $4 damage to a minion. If it dies, add a copy of it to your_hand.
internal static class Sim_GIL_134
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
