using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 招募合约
// Recruitment Contract
// 消灭一个随从。将一张它的复制置入你的手牌。
// Destroy a minion. Add_a copy of it to your hand.
internal static class Sim_DAL_366t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
