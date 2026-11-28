namespace Silverfish.Simulation.Cards;

// 砰砰博士
// Dr. Boom
// 战吼： 召唤两个1/1的砰砰机器人。警告：该机器人随时可能爆炸。
// Battlecry: Summon two 1/1 Boom Bots. WARNING: Bots may explode.
[CardSim("GVG_110")]
internal static class Sim_GVG_110
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonEffect("GVG_110t"), new SummonEffect("GVG_110t")]
    };
}
