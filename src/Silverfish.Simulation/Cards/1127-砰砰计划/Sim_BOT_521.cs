using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 炼魂术
// Ectomancy
// 召唤你控制的所有恶魔的复制。
// Summon copies of all Demons you control.
internal static class Sim_BOT_521
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
