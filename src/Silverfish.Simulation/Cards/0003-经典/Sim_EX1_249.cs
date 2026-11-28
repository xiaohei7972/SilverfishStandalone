namespace Silverfish.Simulation.Cards;

// 迦顿男爵
// Baron Geddon
// 在你的回合结束时，对所有其他角色造成2点伤害。
// At the end of your turn, deal 2 damage to ALL other characters.
[CardSim("EX1_249")]
internal static class Sim_EX1_249
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.OtherCharacters, 2)])]
    };
}
