using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迷情护符
// Amulet of Passions
// 直到你的回合结束，夺取一个敌方随从的控制权。（在本回合中其攻击力为1！）
// Take control of an enemy minion until the end of your turn. (It has 1 Attack this turn!)
internal static class Sim_VAC_959t01
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
