using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 洪流
// Torrent
// 对一个随从造成$8点伤害。如果你在上个回合施放过法术，则法力值消耗减少（3）点。
// Deal $8 damage to a minion. Costs (3) less if you cast a spell last turn.
internal static class Sim_BT_110
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
