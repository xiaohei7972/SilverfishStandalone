using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 无穷永动机
// PMM Infinitizer
// 战吼：将一个友方随从的攻击力和生命值变为8。该随从在本回合中无法攻击英雄。
// Battlecry: Set a friendly minion's Attack and Health to 8. It can't attack heroes this turn.
internal static class Sim_TIME_043
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
