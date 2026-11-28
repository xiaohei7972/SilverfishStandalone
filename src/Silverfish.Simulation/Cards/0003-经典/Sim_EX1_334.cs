using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暗影狂乱
// Shadow Madness
// 直到回合结束，获得一个攻击力小于或等于3的敌方随从的控制权。
// Gain control of an enemy minion with 3 or less Attack until end of turn.
[CardSim("EX1_334")]
internal static class Sim_EX1_334
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_MAX_ATTACK, 3), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new TakeControlEffect(ReadyToAttack: true),
            Sim_EX1_334e.Rule.ApplyTo(StatTarget.Selected)
        ]
    };
}
