namespace Silverfish.Simulation.Cards;

// 麦迪文的残影
// Echo of Medivh
// 将每个友方随从的各一张复制置入你的手牌。
// Put a copy of each friendly minion into your hand.
[CardSim("GVG_005")]
internal static class Sim_GVG_005
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new CopyFriendlyBoardToHandEffect()]
    };
}
