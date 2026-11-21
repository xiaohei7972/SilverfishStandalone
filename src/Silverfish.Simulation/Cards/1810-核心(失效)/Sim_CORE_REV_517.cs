using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 罪犯列队
// Criminal Lineup
// 选择一个友方随从。召唤3个它的复制。过载：（2）
// Choose a friendly minion. Summon 3 copies of it. Overload: (2)
internal static class Sim_CORE_REV_517
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
