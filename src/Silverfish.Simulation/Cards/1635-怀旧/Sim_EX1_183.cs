using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 野性赐福
// Gift of the Wild
// 使你的所有随从获得+2/+2和 嘲讽。
// Give your minions +2/+2 and Taunt.
internal static class Sim_EX1_183
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_TOTAL_MINIONS, 2)],
        []);
    // </legacy-play-requirements>

}
