using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暮光召唤
// Twilight's Call
// 召唤两个在本局对战中死亡，并具有亡语的友方随从的1/1复制。
// Summon 1/1 copies of 2 friendly Deathrattle minions that died this game.
internal static class Sim_LOOT_187
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_GAME)],
        []);
    // </legacy-play-requirements>

}
