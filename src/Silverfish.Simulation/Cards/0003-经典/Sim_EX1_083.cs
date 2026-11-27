namespace Silverfish.Simulation.Cards;

// 工匠大师欧沃斯巴克
// Tinkmaster Overspark
// 战吼： 随机使另一个随从变形成为一个5/5的魔暴龙或一个1/1的松鼠。
// Battlecry: Transform another random minion into a 5/5 Devilsaur or a 1/1 Squirrel.
[CardSim("EX1_083")]
internal static class Sim_EX1_083
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new RandomTransformOtherMinionEffect(["EX1_tk28", "EX1_tk29"])]
    };
}
