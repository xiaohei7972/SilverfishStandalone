using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 看台喷火龙
// Crowd Roaster
// 战吼： 如果你的手牌中有龙牌，则对一个敌方随从造成7点伤害。
// Battlecry: If you're holding a Dragon, deal 7 damage to an enemy minion.
internal static class Sim_TRL_569
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
