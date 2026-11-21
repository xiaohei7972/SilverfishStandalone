using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 塔达拉姆王子
// Prince Taldaram
// 战吼：如果你的牌库里没有法力值消耗为（3）的牌，则变形成为一个随从的3/3的复制。
// Battlecry: If your deck has_no 3-Cost cards, transform into a 3/3 copy of a minion.
internal static class Sim_ICC_852
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
