using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 血誓雇佣兵
// Bloodsworn Mercenary
// 战吼：选择一个受伤的友方随从，召唤一个它的复制。
// Battlecry: Choose a damaged friendly minion. Summon a copy of it.
internal static class Sim_ULD_720
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_DAMAGED_TARGET)],
        []);
    // </legacy-play-requirements>

}
