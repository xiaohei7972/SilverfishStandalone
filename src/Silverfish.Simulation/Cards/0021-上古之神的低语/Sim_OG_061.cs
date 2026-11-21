using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 搜寻猎物
// On the Hunt
// 造成$1点伤害。召唤一个1/1的獒犬。
// Deal $1 damage. Summon a 1/1 Mastiff.
internal static class Sim_OG_061
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
