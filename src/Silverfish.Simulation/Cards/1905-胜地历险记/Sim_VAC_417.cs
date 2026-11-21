using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 感官侵夺
// Sensory Deprivation
// 召唤一个敌方随从的一个复制。如果你的生命值小于或等于20点，消灭本体。
// Summon a copy of an enemy minion. If you have 20 or less Health, destroy the original.
internal static class Sim_VAC_417
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
