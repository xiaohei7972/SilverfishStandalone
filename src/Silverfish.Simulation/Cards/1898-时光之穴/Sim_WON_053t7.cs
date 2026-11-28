using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大地神殿
// Temple of Earth
// 选择一个友方随从。召唤一个它的具有+3生命值和嘲讽且无法攻击的复制。
// Choose a friendly minion. Summon a copy of it with +3_Health and Taunt that can't attack.
internal static class Sim_WON_053t7
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
