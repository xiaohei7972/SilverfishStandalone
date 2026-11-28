namespace Silverfish.Simulation.Cards;

// 恶鳍审判者
// Vilefin Inquisitor
// 战吼： 你的英雄技能变为“召唤一个1/1的鱼人”。
// Battlecry: Your Hero Power becomes 'Summon a 1/1 Murloc.'
[CardSim("OG_006")]
internal static class Sim_OG_006
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ReplaceHeroPowerEffect("OG_006b"),
        ],
    };
}
