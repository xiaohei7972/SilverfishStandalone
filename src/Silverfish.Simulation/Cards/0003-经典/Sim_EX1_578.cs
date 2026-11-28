using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 野蛮之击
// Savagery
// 对一个随从造成等同于你的英雄攻击力的伤害。
// Deal damage equal to your hero's Attack to a minion.
[CardSim("EX1_578")]
internal static class Sim_EX1_578
{
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ScaledDamageEffect(ConditionCharacter.FriendlyHero, CharacterMetric.Attack, DamageTarget.Selected)]
    };
}
