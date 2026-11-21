using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 专注意志
// Focused Will
// 沉默一个随从，然后使其获得+3生命值。
// Silence a minion, then give it +3 Health.
internal static class Sim_CS3_027
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
