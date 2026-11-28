using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 砰砰飞艇
// The Boomship
// 随机从你的手牌中召唤三个随从，并使其获得突袭。
// Summon 3 random minions from your hand. Give them Rush.
internal static class Sim_BOT_069
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
