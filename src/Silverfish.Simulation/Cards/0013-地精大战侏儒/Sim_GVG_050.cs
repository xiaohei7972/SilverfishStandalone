using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 弹射之刃
// Bouncing Blade
// 随机对一个随从造成$1点伤害。重复此效果，直到某个随从死亡。
// Deal $1 damage to a random minion. Repeat until a minion dies.
internal static class Sim_GVG_050
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_TOTAL_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
