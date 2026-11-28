using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣之灵
// Divine Spirit
// 使一个随从的生命值翻倍。
// Double a minion's Health.
internal static class Sim_CS2_236
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
