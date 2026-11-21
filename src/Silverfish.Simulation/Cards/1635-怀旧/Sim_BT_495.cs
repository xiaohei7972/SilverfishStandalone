using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 刃缚精锐
// Glaivebound Adept
// 战吼：在本回合中，如果你的英雄进行过攻击，则造成4点 伤害。
// Battlecry: If your hero attacked this turn, deal 4 damage.
internal static class Sim_BT_495
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_NONSELF_TARGET)],
        []);
    // </legacy-play-requirements>

}
