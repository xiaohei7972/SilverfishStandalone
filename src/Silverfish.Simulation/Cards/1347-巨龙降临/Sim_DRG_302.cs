using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 墓地符文
// Grave Rune
// 使一个随从获得“亡语：召唤该随从的两个复制。”
// Give a minion "Deathrattle: Summon 2 copies of this."
internal static class Sim_DRG_302
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
