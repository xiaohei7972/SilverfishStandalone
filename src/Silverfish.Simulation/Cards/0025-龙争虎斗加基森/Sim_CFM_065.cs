namespace Silverfish.Simulation.Cards;

// 火山药水
// Volcanic Potion
// 对所有随从造成$2点伤害。
// Deal $2 damage to all_minions.
[CardSim("CFM_065")]
internal static class Sim_CFM_065
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.AllMinions, 2)],
    };
}
