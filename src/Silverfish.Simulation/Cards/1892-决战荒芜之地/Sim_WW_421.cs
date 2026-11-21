using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 帮派头领普德
// Kingpin Pud
// 战吼：复活你的食人魔帮众，使其获得 风怒。
// Battlecry: Resurrect your Ogre-Gang. Give them Windfury.
internal static class Sim_WW_421
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_GAME)],
        []);
    // </legacy-play-requirements>

}
