using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 好奇的萤根草
// Curious Glimmerroot
// 战吼：检视三张卡牌。猜中来自你对手套牌中的卡牌，则获取该牌的一张复制。
// Battlecry: Look at 3 cards. Guess which one started in your opponent's deck to get a copy of it.
internal static class Sim_UNG_035
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
