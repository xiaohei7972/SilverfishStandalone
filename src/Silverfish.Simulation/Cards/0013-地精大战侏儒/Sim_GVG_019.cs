using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恶魔之心
// Demonheart
// 对一个随从造成$5点伤害，如果该随从是友方恶魔，则改为使其获得+5/+5。
// Deal $5 damage to a minion. If it's a friendly Demon, give it +5/+5 instead.
internal static class Sim_GVG_019
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
