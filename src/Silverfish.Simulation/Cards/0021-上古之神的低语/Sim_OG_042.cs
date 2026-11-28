namespace Silverfish.Simulation.Cards;

// 亚煞极
// Y'Shaarj, Rage Unbound
// 在你的回合结束时，将一个随从从你的牌库置入战场。
// At the end of your turn, put a minion from your deck into the battlefield.
[CardSim("OG_042")]
internal static class Sim_OG_042
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new RecruitEffect()]),
        ],
    };
}
