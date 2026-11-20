namespace Silverfish.Simulation.Cards;

// 年轻的酒仙
// Youthful Brewmaster
// 战吼：使一个友方随从从战场上移回你的手牌。
// Battlecry: Return a friendly minion from the battlefield to your hand.
[CardSim("EX1_049")]
internal static class Sim_EX1_049
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.FriendlyMinion,
        OnPlay = [new ReturnMinionEffect()]
    };
}
