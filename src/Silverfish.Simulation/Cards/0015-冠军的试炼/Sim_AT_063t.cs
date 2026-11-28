namespace Silverfish.Simulation.Cards;

// 恐鳞
// Dreadscale
// 在你的回合结束时，对所有敌人造成 1点伤害。
// At the end of your turn, deal 1 damage to all enemies.
[CardSim("AT_063t")]
internal static class Sim_AT_063t
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.AllEnemies, 1)]),
        ],
    };
}
