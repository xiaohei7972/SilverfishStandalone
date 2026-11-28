namespace Silverfish.Simulation.Cards;

// 加里维克斯
// Trade Prince Gallywix
// 每当你的对手施放一个法术，获得该法术的复制，并使其获得一张幸运币。
// Whenever your opponent casts a spell, gain a copy of it and give them a Coin.
[CardSim("GVG_028")]
internal static class Sim_GVG_028
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.BeforeSpellResolved,
                TriggerRelation.Opponent,
                [
                    new CopyTriggerSpellToOtherPlayerEffect(),
                    new CreateCardEffect("GVG_028t", Recipient: CardRecipient.Opponent)
                ])
            {
                ExcludeCardId = "GVG_028t"
            }
        ]
    };
}
