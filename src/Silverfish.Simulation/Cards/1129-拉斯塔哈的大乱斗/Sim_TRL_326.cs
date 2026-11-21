using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 燃棘枪兵
// Smolderthorn Lancer
// 战吼：如果你的手牌中有龙牌，则消灭一个受伤的敌方随从。
// Battlecry: If you're holding a Dragon, destroy a damaged enemy minion.
internal static class Sim_TRL_326
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND), new(ErrorType.REQ_DAMAGED_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
