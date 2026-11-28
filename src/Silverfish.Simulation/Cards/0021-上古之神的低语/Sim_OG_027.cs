using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 异变
// Evolve
// 随机将你的 所有随从变形成为法力值消耗增加（1）点的随从。
// Transform your minions into random minions that cost (1) more.
[CardSim("OG_027")]
internal static class Sim_OG_027
{
    internal static PlayReqSet PlayReqs { get; } = new([], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new TransformFriendlyMinionsByCostEffect(1),
        ],
    };
}
