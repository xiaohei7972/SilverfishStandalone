using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 温顺的巨壳龙
// Gentle Megasaur
// 战吼：进化你所有的鱼人。
// Battlecry: Adapt your Murlocs.
[CardSim("UNG_089")]
internal static class Sim_UNG_089
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new AdaptChoice(AdaptChoice.StandardOptions, AdaptRecipient.FriendlyMinions,
            new CardFilter { MinionType = MinionType.Murloc })
    };
}
