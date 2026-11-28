using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 旅行管理员杜加尔
// Travelmaster Dungar
// 战吼：从你的牌库中召唤三个来自不同扩展包的随从。
// Battlecry: Summon three minions from your deck that are from different expansions.
internal static class Sim_WORK_043
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 3)],
        []);
    // </legacy-play-requirements>

}
