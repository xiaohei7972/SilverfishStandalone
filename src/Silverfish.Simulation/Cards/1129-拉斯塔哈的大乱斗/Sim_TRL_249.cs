using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 残酷集结
// Grim Rally
// 消灭一个友方随从。使你的所有随从获得+1/+1。
// Destroy a friendly minion. Give your minions +1/+1.
internal static class Sim_TRL_249
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
