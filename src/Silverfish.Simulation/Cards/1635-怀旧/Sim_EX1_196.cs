using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 血色征服者
// Scarlet Subjugator
// 战吼：直到你的下个回合，使一个敌方随从获得-2攻击力。
// Battlecry: Give an enemy minion -2 Attack until your_next turn.
internal static class Sim_EX1_196
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
