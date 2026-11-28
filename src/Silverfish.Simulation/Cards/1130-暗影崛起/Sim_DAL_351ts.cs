using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 远古祝福
// Blessing of the Ancients
// 使你的所有随从获得+1/+1。
// Give your minions +1/+1.
internal static class Sim_DAL_351ts
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS, 2)],
        []);
    // </legacy-play-requirements>

}
