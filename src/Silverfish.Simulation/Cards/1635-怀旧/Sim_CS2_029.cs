using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火球术
// Fireball
// 造成$6点伤害。
// Deal $6 damage.
[CardSim("CS2_029")]
internal static class Sim_CS2_029
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Selected, 6)]
    };

}
