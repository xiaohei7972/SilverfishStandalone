using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 碎枝
// Splintergraft
// 战吼：选择一个友方随从。将它的一张10/10复制置入你的手牌，其法力值消耗为（10）点。
// Battlecry: Choose a friendly minion. Add a 10/10 copy to your hand that costs (10).
internal static class Sim_GIL_658
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
