using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奥妮克希亚鳞片
// Scale of Onyxia
// 用2/1并具有突袭的雏龙填满你的面板。
// Fill your board with 2/1 Whelps with Rush.
internal static class Sim_ONY_021
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
