using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 纵火指控
// Arson Accusation
// 选择一个随从，在你的英雄受到伤害后将其消灭。
// Choose a minion. Destroy it after your hero takes damage.
internal static class Sim_CORE_MAW_001
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
