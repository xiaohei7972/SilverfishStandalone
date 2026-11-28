using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 结网蛛
// Webspinner
// 亡语：随机获取一张野兽牌。
// Deathrattle: Get a random Beast.
[CardSim("FP1_011")]
internal static class Sim_FP1_011
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new CreateRandomMatchingCardEffect(new()
            {
                Kind = EntityKind.Minion,
                MinionType = MinionType.Beast
            })
        ]
    };
}
