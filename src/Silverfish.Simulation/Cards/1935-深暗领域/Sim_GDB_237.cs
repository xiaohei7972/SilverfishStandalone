using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 接触异星生物
// Alien Encounters
// 召唤两只2/5并具有嘲讽的野兽。在本局对战中你每发现过一张牌，本牌的法力值消耗便减少（1）点。
// Summon two 2/5 Beasts with Taunt. Costs (1) less for each card you Discovered this game.
internal static class Sim_GDB_237
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
