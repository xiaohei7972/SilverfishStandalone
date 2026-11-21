using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 群体复活
// Mass Resurrection
// 召唤三个在本局对战中死亡的友方随从。
// Summon 3 friendly minions that died this game.
internal static class Sim_DAL_724
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_GAME)],
        []);
    // </legacy-play-requirements>

}
