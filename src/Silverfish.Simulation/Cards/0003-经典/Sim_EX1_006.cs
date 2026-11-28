namespace Silverfish.Simulation.Cards;

// 报警机器人
// Alarm-o-Bot
// 在你的回合开始时，随机将你的手牌中的一张随从牌与本随从 交换。
// At the start of your turn, swap this minion with a random one in your hand.
[CardSim("EX1_006")]
internal static class Sim_EX1_006
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new SwapSelfWithRandomHandMinionEffect()])]
    };
}
