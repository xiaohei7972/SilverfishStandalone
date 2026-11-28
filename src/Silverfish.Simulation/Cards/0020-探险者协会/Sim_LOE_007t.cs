namespace Silverfish.Simulation.Cards;

// 诅咒
// Cursed!
// 如果这张牌在你的手牌中，在你的回合开始时，你的英雄受到2点伤害。
// While this is in your hand, take 2 damage at the start of your turn.
[CardSim("LOE_007t")]
internal static class Sim_LOE_007t
{
    internal static CardRules Rules { get; } = new()
    {
        InHandTriggers =
        [
            new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.FriendlyHero, 2)]),
        ],
    };
}
