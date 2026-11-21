using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 泰坦考据学家欧斯克
// Titanographer Osk
// 战吼：选择一个非泰坦随从，召唤一个具有+2/+2的复制。
// Battlecry: Choose a non-Titan minion. Summon a copy of it with +2/+2.
internal static class Sim_TLC_452t19
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IS_NON_TITAN)],
        []);
    // </legacy-play-requirements>

}
