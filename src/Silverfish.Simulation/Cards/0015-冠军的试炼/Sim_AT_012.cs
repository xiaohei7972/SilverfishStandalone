namespace Silverfish.Simulation.Cards;

// 暗影子嗣
// Spawn of Shadows
// 战吼，激励：对每个英雄造成4点伤害。
// Battlecry and Inspire: Deal 4 damage to each hero.
[CardSim("AT_012")]
internal static class Sim_AT_012
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.BothHeroes, 4),
        ],
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.BothHeroes, 4)]),
        ],
    };
}
