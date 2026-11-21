using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 控心术
// Mindgames
// 随机将你对手的牌库中的一张随从牌的复制置入战场。
// Put a copy of a random minion from your opponent's deck into the battlefield.
internal static class Sim_VAN_EX1_345
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
