using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 殊死一搏
// Desperate Stand
// 使一个随从获得“亡语：回到战场，并具有1点生命值。”
// Give a minion "Deathrattle: Return this to life with 1 Health."
internal static class Sim_ICC_244
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
