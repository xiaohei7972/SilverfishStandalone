namespace Silverfish.Simulation.Cards;

// 虚灵商人
// Ethereal Peddler
// 战吼：如果你的手牌中有另一职业的卡牌，则其法力值消耗减少（2）点。
// Battlecry: If you're holding any cards from another class, reduce their Cost by (2).
[CardSim("KAR_070")]
internal static class Sim_KAR_070
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeHandCostEffect(HandCostTarget.FriendlyHand, -2) { OtherClassOnly = true }],
    };
}
