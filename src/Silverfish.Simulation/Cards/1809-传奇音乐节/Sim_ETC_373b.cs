using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 美妙振动
// Good Vibrations
// 使你的所有随从获得+2/+4和 嘲讽。
// Give your minions +2/+4 and Taunt.
internal static class Sim_ETC_373b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_TOTAL_MINIONS, 2)],
        []);
    // </legacy-play-requirements>

}
