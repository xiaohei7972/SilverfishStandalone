using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 园地管理员
// Groundskeeper
// 嘲讽。战吼：如果你的手牌中有法力值消耗大于或等于（5）点的法术牌，恢复#5点生命值。
// Taunt Battlecry: If you're holding a spell that costs (5) or more, restore #5 Health.
internal static class Sim_SCH_613
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
