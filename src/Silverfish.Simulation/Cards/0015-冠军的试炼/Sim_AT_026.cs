namespace Silverfish.Simulation.Cards;

// 愤怒卫士
// Wrathguard
// 每当本随从受到伤害，对你的英雄造成等量的伤害。
// Whenever this minion takes damage, also deal that amount to your hero.
[CardSim("AT_026")]
internal static class Sim_AT_026
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.Damage, TriggerRelation.Friendly, [new EventAmountDamageEffect(DamageTarget.FriendlyHero)]) { EntityMatch = TriggerEntityMatch.Self },
        ],
    };
}
