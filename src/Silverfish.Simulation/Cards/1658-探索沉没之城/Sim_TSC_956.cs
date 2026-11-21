using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 拖入深渊
// Dragged Below
// 对一个随从造成$4点伤害。使你的对手获得一张深渊诅咒。
// Deal $4 damage to a minion. Give your opponent an Abyssal Curse.
internal static class Sim_TSC_956
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
