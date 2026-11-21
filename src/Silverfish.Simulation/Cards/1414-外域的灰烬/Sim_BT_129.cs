using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 萌芽分裂
// Germination
// 召唤一个友方随从的复制。使复制获得嘲讽。
// Summon a copy of a friendly minion. Give the copy Taunt.
internal static class Sim_BT_129
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
