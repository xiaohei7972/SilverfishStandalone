using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗言术：痛
// Shadow Word: Pain
// 消灭一个攻击力小于或等于3的随从。
// Destroy a minion with 3 or less Attack.
[CardSim("CS2_234")]
internal static class Sim_CS2_234
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_MAX_ATTACK, 3), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyMinionEffect()]
    };
}
