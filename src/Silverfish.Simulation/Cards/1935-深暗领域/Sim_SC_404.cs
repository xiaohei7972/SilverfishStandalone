using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 回收地堡
// Salvage the Bunker
// 召唤两个2/2并具有嘲讽的陆战队员。你的下一次星舰发射的法力值消耗减少（2）点。
// Summon two 2/2 Marines with Taunt. Your next Starship launch costs (2) less.
internal static class Sim_SC_404
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
