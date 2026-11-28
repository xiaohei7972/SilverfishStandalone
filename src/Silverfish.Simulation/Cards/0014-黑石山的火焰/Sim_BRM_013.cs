using Silverfish.Domain;
using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 快速射击
// Quick Shot
// 造成$3点伤害。 如果你没有其他手牌，则抽一张牌。
// Deal $3 damage. If your hand is empty, draw a card.
[CardSim("BRM_013")]
internal static class Sim_BRM_013
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.Selected, 3),
            new ConditionalEffect(
                new ZoneCountCondition(
                    ConditionPlayer.Friendly,
                    GameZone.Hand,
                    NumberComparison.Equal,
                    0),
                [new DrawEffect(1)])
        ]
    };
}
