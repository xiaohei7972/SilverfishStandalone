using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 诛灭暴君
// Dethrone
// 消灭一个随从。连击：随机召唤一个法力值消耗为（8）的随从。
// Destroy a minion. Combo: Summon a random 8-Cost minion.
internal static class Sim_TIME_712
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
