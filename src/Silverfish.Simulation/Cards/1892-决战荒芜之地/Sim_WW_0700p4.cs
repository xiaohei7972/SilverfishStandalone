using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣枪弹
// Holy Bullet
// 造成$2点伤害。随机使一个友方随从获得+2/+2。每回合切换。
// Deal $2 damage. Give a random friendly minion +2/+2. Swaps each turn.
internal static class Sim_WW_0700p4
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
