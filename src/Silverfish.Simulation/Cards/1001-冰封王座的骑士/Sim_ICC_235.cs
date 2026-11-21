using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影精华
// Shadow Essence
// 随机挑选你牌库里的一个随从，召唤一个5/5的复制。
// Summon a 5/5 copy of_a random minion in_your deck.
internal static class Sim_ICC_235
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
