using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 精神控制技师
// Mind Control Tech
// 战吼：如果你的对手有4个或者更多随从，夺取其中一个的控制权。
// Battlecry: If your opponent has 4 or more minions, take control of one.
internal static class Sim_CORE_EX1_085
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
