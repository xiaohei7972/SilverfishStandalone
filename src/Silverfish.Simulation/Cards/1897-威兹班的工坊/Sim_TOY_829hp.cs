using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 跃动的南瓜
// Pulsing Pumpkins
// 造成$3点伤害，威力无穷！发现一张亡灵牌，充当仆从！
// Deal $3 damage, with crushing brawn! Discover an Undead, to serve as your pawn!
internal static class Sim_TOY_829hp
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
