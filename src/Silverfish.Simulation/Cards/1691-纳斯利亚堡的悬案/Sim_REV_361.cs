using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 野性之魂
// Wild Spirits
// 召唤两个不同的休眠灵种。使你的灵种提前1回合唤醒。
// Summon two different Dormant Wildseeds. Make your Wildseeds awaken 1 turn sooner.
internal static class Sim_REV_361
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
