using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 刺杀
// Assassinate
// 消灭一个敌方随从。
// Destroy an enemy minion.
[CardSim("CS2_076")]
internal static class Sim_CS2_076
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyMinionEffect()]
    };
}
