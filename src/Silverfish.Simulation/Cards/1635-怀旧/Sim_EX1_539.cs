using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 杀戮命令
// Kill Command
// 造成$3点伤害。如果你控制一个野兽，则改为造成 $5点伤害。
// Deal $3 damage. If you control a Beast, deal $5 damage instead.
internal static class Sim_EX1_539
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
