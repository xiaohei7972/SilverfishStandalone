using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 拦住他们！
// Hold Them Off!
// 预备。使一个随从获得+5/+5和吸血。
// Prepare Give a minion +5/+5 and Lifesteal.
internal static class Sim_JAIL_913
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
