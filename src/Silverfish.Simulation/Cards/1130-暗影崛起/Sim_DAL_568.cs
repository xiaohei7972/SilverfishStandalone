using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 光铸祝福
// Lightforged Blessing
// 双生法术 使一个友方随从获得吸血。
// Twinspell Give a friendly minion Lifesteal.
internal static class Sim_DAL_568
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
