using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 终极巫毒
// Big Bad Voodoo
// 使一个友方随从获得“亡语：随机召唤一个法力值消耗增加（1）点的随从。”
// Give a friendly minion "Deathrattle: Summon a random minion that costs (1) more."
internal static class Sim_TRL_082
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
