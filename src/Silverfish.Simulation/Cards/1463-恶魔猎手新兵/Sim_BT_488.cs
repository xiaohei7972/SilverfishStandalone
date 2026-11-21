using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灵魂分裂
// Soul Split
// 选择一个友方恶魔，召唤一个它的复制。
// Choose a friendly Demon. Summon a copy of it.
internal static class Sim_BT_488
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DEMON), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
