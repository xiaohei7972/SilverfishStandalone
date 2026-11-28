using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 吸血鬼之牙
// Vampiric Fangs
// 消灭一个随从。为你的英雄恢复生命值，数值相当于该随从的生命值。
// Destroy a minion. Restore its Health to your hero.
internal static class Sim_VAC_464t28
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
