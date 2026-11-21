using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 佐克无敌！
// Zok Rocks!
// 使一个友方随从获得+2攻击力和突袭。
// Give a friendly minion +2 Attack and Rush.
internal static class Sim_JAM_027a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
