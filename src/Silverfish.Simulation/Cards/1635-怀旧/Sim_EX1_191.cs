using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 瘟疫使者
// Plaguebringer
// 战吼：使一个友方随从获得剧毒。
// Battlecry: Give a friendly minion Poisonous.
internal static class Sim_EX1_191
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
