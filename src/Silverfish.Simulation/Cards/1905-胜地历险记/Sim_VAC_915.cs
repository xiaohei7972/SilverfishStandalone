using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大力扣杀
// Power Spike
// 造成$4点伤害。随机使一个友方随从获得+4/+4。
// Deal $4 damage. Give a random friendly minion +4/+4.
internal static class Sim_VAC_915
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
