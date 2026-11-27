namespace Silverfish.Simulation.Cards;

// 光明之泉
// Lightwell
// 在你的回合开始时，随机为一个受伤的 友方角色恢复#3点生命值。
// At the start of your turn, restore #3 Health to a damaged friendly character.
[CardSim("EX1_341")]
internal static class Sim_EX1_341
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new HealEffect(HealTarget.RandomDamagedFriendlyCharacter, 3)])]
    };
}
