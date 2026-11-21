using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 谐振盘
// Resonance Coil
// 对一个随从造成$5点伤害。随机获取一张星灵法术牌。
// Deal $5 damage to a minion. Get a random Protoss spell.
internal static class Sim_SC_760
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
