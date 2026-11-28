using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雷霆之神高戈奈斯
// Golganneth, the Thunderer
// 泰坦 你每个回合使用的第一张法术牌的法力值消耗减少（3）点。
// Titan Your first spell each turn costs (3) less.
internal static class Sim_TTN_800
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
