using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 拟态豆荚
// Mimic Pod
// 抽一张牌，然后将一张它的复制置入你的手牌。
// Draw a card, then add a copy of it to your hand.
internal static class Sim_UNG_060
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
