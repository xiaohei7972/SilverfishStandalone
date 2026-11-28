using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;
namespace Silverfish.Simulation.Cards;

// 赤红深渊
// Sanguine Depths
// 对一个随从造成1点伤害，并使其获得+2攻击力。
// Deal 1 damage to a minion and give it +2 Attack.
[CardSim("REV_990")]
internal static class Sim_REV_990
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        Activation = new([new DamageEffect(DamageTarget.Selected, 1), Sim_REV_990e.Rule.ApplyTo(StatTarget.Selected)])
    };
}
