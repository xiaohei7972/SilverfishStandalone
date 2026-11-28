using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 饥饿的海怪
// Ravenous Kraken
// 战吼：消灭一个友方随从。亡语：召唤一个被消灭随从的复制。
// Battlecry: Destroy a friendly minion. Deathrattle: Summon a copy of it.
internal static class Sim_TTN_754
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
