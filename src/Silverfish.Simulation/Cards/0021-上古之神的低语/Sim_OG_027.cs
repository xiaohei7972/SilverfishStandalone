using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 异变
// Evolve
// 随机将你的 所有随从变形成为法力值消耗增加（1）点的随从。
// Transform your minions into random minions that cost (1) more.
internal static class Sim_OG_027
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
