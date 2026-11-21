using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 血虫感染
// Sanguine Infestation
// 抽两张牌。召唤两条0/2的水蛭。
// Draw 2 cards. Summon two 0/2 Leeches.
internal static class Sim_EDR_817
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
