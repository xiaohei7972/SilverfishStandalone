using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 科多兽坐骑
// Kodo Mount
// 使一个随从获得+4/+2和突袭。当该随从死亡时，召唤一只科多兽。
// Give a minion +4/+2 and Rush. When it dies, summon a Kodo.
internal static class Sim_SW_432
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
