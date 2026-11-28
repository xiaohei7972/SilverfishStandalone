namespace Silverfish.Simulation.Cards;

// 迪菲亚头目
// Defias Ringleader
// 连击：&召唤一个2/1的迪菲亚强盗。
// Combo:& Summon a 2/1 Defias Bandit.
[CardSim("EX1_131")]
internal static class Sim_EX1_131
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ConditionalEffect(new ComboCondition(), [new SummonEffect("EX1_131t")])]
    };
}
