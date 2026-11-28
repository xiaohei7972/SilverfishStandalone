namespace Silverfish.Simulation.Cards;

// 极恶之咒
// Excavated Evil
// 对所有随从造成$3点伤害。将该牌洗入你对手的牌库。
// Deal $3 damage to all minions. Shuffle this card into your opponent's deck.
[CardSim("LOE_111")]
internal static class Sim_LOE_111
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.AllMinions, 3),
            new ShufflePlayedCardEffect(CardRecipient.Opponent),
        ],
    };
}
