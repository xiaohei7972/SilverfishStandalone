using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 服装裁缝
// Outfit Tailor
// 战吼：使一个友方随从获得等同于本随从的攻击力和生命值。
// Battlecry: Give a friendly minion Attack and Health equal to this minion's.
internal static class Sim_ETC_420
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
