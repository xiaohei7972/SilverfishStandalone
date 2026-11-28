namespace Silverfish.Simulation.Cards;

// 麻风侏儒
// Leper Gnome
// 亡语：对敌方英雄造成2点伤害。
// Deathrattle: Deal 2 damage to the enemy_hero.
[CardSim("EX1_029")]
internal static class Sim_EX1_029
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new DamageEffect(DamageTarget.EnemyHero, 2)]
    };
}
