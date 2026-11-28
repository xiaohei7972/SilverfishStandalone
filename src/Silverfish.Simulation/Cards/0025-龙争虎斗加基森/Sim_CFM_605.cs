namespace Silverfish.Simulation.Cards;

// 龙人侦测者
// Drakonid Operative
// 战吼： 如果你的手牌中有龙牌，便发现你对手牌库中一张牌的复制。
// Battlecry: If you're holding a Dragon, Discover a copy of a card in your opponent's deck.
[CardSim("CFM_605")]
internal static class Sim_CFM_605
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice
        {
            SourceDeck = ConditionPlayer.Opponent,
            Condition = new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Hand,
                NumberComparison.AtLeast, 1, EntityKind.Minion)
            {
                CardFilter = new CardFilter { MinionType = MinionType.Dragon },
            },
        },
    };
}
