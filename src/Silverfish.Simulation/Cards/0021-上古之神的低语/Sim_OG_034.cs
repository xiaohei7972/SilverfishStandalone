namespace Silverfish.Simulation.Cards;

// 异种群居蝎
// Silithid Swarmer
// 在本回合中，除非你的英雄进行过攻击，否则无法进行攻击。
// Can only attack if your hero attacked this turn.
[CardSim("OG_034")]
internal static class Sim_OG_034
{
    internal static CardRules Rules { get; } = new()
    {
        RequiresHeroAttacked = true,
    };
}
