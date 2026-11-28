using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恋旧风潮
// Wave of Nostalgia
// 将所有随从变形成为来自过去的随机传说随从。
// Transform ALL minions into random Legendary ones from the past.
internal static class Sim_MIS_701
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
