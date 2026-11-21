using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗处的身影
// Figure in the Dark
// 召唤两个3/6并具有嘲讽且无法攻击的恶魔。
// Summon two 3/6 Demons with Taunt that can't attack.
internal static class Sim_EDR_490a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
