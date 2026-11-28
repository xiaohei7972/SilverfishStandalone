namespace Silverfish.Simulation.Cards;

// 灌魔之锤
// Charged Hammer
// 亡语：你的英雄技能改为“造成 2点伤害”。
// Deathrattle: Your Hero Power becomes 'Deal 2 damage.'
[CardSim("AT_050")]
internal static class Sim_AT_050
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new ReplaceHeroPowerEffect("AT_050t"),
        ],
    };
}
