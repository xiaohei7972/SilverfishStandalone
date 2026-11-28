namespace Silverfish.Simulation.Cards;

// 裂颅之击
// Headcrack
// 对敌方英雄造成$2点伤害；连击：在下个回合将其移回你的手牌。
// Deal $2 damage to the enemy hero. Combo: Return this to your hand next turn.
[CardSim("EX1_137")]
internal static class Sim_EX1_137
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.EnemyHero, 2),
            new ConditionalEffect(new ComboCondition(), [new CreateCardAtTurnEndEffect("EX1_137")])
        ]
    };
}
