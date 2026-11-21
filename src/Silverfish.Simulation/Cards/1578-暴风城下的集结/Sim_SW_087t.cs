using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 塔姆辛的恐惧战马
// Tamsin's Dreadsteed
// 亡语：在回合结束时，召唤塔姆辛的恐惧战马。
// Deathrattle: At the end of the turn, summon Tamsin's Dreadsteed.
internal static class Sim_SW_087t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
