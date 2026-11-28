namespace Silverfish.Simulation.Cards;

// 伊瑟拉
// Ysera
// 在你的回合结束时，随机获取两张梦境牌。
// At the end of your turn, get two random Dream cards.
[CardSim("EX1_572")]
internal static class Sim_EX1_572
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly,
                [new CreateRandomCardEffect(["DREAM_01", "DREAM_02", "DREAM_03", "DREAM_04", "DREAM_05"], Count: 2)])
        ]
    };
}
