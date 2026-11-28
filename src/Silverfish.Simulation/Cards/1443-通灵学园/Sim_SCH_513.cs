using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 脆骨破坏者
// Brittlebone Destroyer
// 战吼：在本回合中，如果你的英雄的生命值发生变化，消灭一个随从。
// Battlecry: If your hero's Health changed this turn, destroy a minion.
internal static class Sim_SCH_513
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
