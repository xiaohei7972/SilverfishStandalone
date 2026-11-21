using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 跃动的南瓜
// Pulsing Pumpkins
// 造成$3点伤害，威力无穷！找到头颅，我的部队向你效忠！
// Deal $3 damage, with crushing brawn! My army shall be yours, if my Head is drawn!
internal static class Sim_TOY_829hp3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
