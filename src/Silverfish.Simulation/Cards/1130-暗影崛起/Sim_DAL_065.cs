using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 不眠之魂
// Unsleeping Soul
// 沉默一个友方随从，然后召唤一个它的复制。
// Silence a friendly minion, then summon a copy of it.
internal static class Sim_DAL_065
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
