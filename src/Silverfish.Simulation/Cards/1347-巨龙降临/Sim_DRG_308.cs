using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 夺心者卡什
// Mindflayer Kaahrj
// 战吼：选择一个敌方随从。亡语：召唤一个所选随从的新的 复制。
// Battlecry: Choose an enemy minion. Deathrattle: Summon a new copy of it.
internal static class Sim_DRG_308
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
