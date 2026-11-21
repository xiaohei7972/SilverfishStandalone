using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 瘟疫科学家
// Plague Scientist
// 连击：使一个友方随从获得剧毒。
// Combo: Give a friendly minion Poisonous.
internal static class Sim_CORE_ICC_809
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_FOR_COMBO)],
        []);
    // </legacy-play-requirements>

}
