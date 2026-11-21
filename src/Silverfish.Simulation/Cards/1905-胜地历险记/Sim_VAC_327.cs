using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰冷整脊师
// Cryopractor
// 战吼：使一个随从获得+3/+3并使其冻结。
// Battlecry: Give a minion +3/+3 and Freeze it.
internal static class Sim_VAC_327
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
