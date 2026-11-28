namespace Silverfish.Simulation.Cards;

// 远古诅咒
// Ancient Curse
// 抽到时施放 你受到7点伤害。
// Casts When Drawn You take 7 damage.
[CardSim("LOE_110t")]
internal static class Sim_LOE_110t
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.FriendlyHero, 7),
        ],
    };
}
