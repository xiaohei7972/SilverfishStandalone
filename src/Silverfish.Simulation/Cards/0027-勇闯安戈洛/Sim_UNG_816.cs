using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 卡利莫斯的仆从
// Servant of Kalimos
// 战吼：如果你在上个回合使用过元素牌，则发现一张元素牌。
// Battlecry: If you played an Elemental last turn, _Discover an Elemental.
internal static class Sim_UNG_816
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE)],
        []);
    // </legacy-play-requirements>

}
