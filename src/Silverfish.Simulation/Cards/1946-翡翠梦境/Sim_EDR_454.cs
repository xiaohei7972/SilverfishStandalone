using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 腐蚀之巢
// Clutch of Corruption
// 选择一条友方的龙。召唤一枚0/2的可以孵化成所选龙的复制的龙蛋。
// Choose a friendly Dragon. Summon a 0/2 Egg that hatches into a copy of it.
internal static class Sim_EDR_454
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DRAGON), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)]);
    // </legacy-play-requirements>

}
