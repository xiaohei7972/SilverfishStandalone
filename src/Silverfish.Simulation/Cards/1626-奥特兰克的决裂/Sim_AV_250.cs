using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雪球大战
// Snowball Fight!
// 对一个随从造成$1点伤害并使其冻结。如果该随从依然存活，则对另一个随从重复此效果！
// Deal $1 damage to a minion and Freeze it. If it survives, repeat this on another minion!
internal static class Sim_AV_250
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_TOTAL_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
