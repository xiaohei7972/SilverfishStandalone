namespace Silverfish.Simulation.Cards;

// 光耀之主拉格纳罗斯
// Ragnaros, Lightlord
// 在你的回合结束时，为一个受伤的友方角色恢复#8点生命值。
// At the end of your turn, restore #8 Health to a damaged friendly character.
[CardSim("OG_229")]
internal static class Sim_OG_229
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new HealEffect(HealTarget.RandomDamagedFriendlyCharacter, 8)]),
        ],
    };
}
