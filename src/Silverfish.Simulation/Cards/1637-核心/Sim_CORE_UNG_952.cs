using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 剑龙骑术
// Spikeridged Steed
// 使一个随从获得+2/+6和嘲讽。当该随从死亡时，召唤一只剑龙。
// Give a minion +2/+6 and Taunt. When it dies, summon a Stegodon.
internal static class Sim_CORE_UNG_952
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
