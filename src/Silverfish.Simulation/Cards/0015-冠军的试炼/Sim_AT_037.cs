using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 活体根须
// Living Roots
// 抉择：造成$2点伤害；或者召唤两个1/1的树苗。
// Choose One - Deal $2 damage; or Summon two 1/1 Saplings.
[CardSim("AT_037")]
internal static class Sim_AT_037
{
    internal static PlayReqSet PlayReqs { get; } = new([], []);

    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("AT_037a", [new DamageEffect(DamageTarget.Selected, 2)]) { PlayReqs = [new(ErrorType.REQ_TARGET_TO_PLAY)] },
            new("AT_037b", [new SummonEffect("AT_037t"), new SummonEffect("AT_037t")]) { PlayReqs = [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)] },
        ],
    };
}
