using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 威能祝福
// Blessing of Authority
// 使一个随从获得+8/+8，在本回合中无法攻击英雄。
// Give a minion +8/+8. It_can't attack heroes this turn.
internal static class Sim_SCH_138
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
