using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 海象人渔夫
// Tuskarr Fisherman
// 战吼：使一个友方随从获得法术伤害+1。
// Battlecry: Give a friendly minion Spell Damage +1.
internal static class Sim_ICC_093
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
