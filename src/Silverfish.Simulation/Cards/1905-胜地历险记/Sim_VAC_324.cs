using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 统一着装
// Matching Outfits
// 将一个随从随机变形成为一个法力值消耗增加（1）点的随从，然后召唤一个它的复制。
// Transform a minion into a random one that costs (1) more, then summon a copy of it.
internal static class Sim_VAC_324
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
