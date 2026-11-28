namespace Silverfish.Simulation.Cards;

// 侏儒实验技师
// Gnomish Experimenter
// 战吼： 抽一张牌，如果该牌是随从牌，则将其变形成为一只小鸡。
// Battlecry: Draw a card. If it's a minion, transform it into a Chicken.
[CardSim("GVG_092")]
internal static class Sim_GVG_092
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawAndTransformIfMinionEffect("GVG_092t")]
    };
}
