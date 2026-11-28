using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 百兽之王
// King of Beasts
// 嘲讽 你每控制一只其他野兽，便拥有+1攻击力。
// Taunt Has +1 Attack for each other Beast you control.
[CardSim("GVG_046")]
internal static class Sim_GVG_046
{
    internal static CardRules Rules { get; } = new()
    {
        AttackPerOtherFriendlyMinionType = MinionType.Beast
    };
}
