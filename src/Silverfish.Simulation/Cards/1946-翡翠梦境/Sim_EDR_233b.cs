using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 猎鹰的灵动
// Falcon's Dexterity
// 召唤两只 4/3并具有风怒 的猎鹰。
// Summon two 4/3 Falcons with Windfury.
internal static class Sim_EDR_233b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
