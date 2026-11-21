using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 两栖药剂
// Amphibious Elixir
// 恢复#5点 生命值。发现一张法术牌。
// Restore #5 Health. Discover a spell.
internal static class Sim_WW_080
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
