using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 粉碎之手
// Crushing Hand
// 对一个随从造成$8点伤害。 过载：（3）
// Deal $8 damage to a minion. Overload: (3)
internal static class Sim_LOOT_060
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
