using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 升格卷轴
// Ascendant Scroll
// 随机将一张法师法术牌置入你的手牌。该牌的法力值消耗减少（2）点。
// Add a random Mage spell to your hand. It costs (2) less.
internal static class Sim_ULD_433p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_HAND_NOT_FULL)],
        []);
    // </legacy-play-requirements>

}
