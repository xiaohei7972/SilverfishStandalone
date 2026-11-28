using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 强行透支
// Overdraft
// 可交易 解锁你过载的法力水晶，并造成等量的 伤害。
// Tradeable Unlock your Overloaded Mana Crystals to deal that much damage.
internal static class Sim_SW_114
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
