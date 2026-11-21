using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 魔像的祝福
// Blessing of the Golem
// 召唤一个@/@的植物魔像。
// Summon a @/@ Plant_Golem.
internal static class Sim_EDR_847p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
