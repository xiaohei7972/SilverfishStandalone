using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 血怒药水
// Bloodfury Potion
// 使一个随从获得+3攻击力。如果该随从是恶魔，还会获得+3生命值。
// Give a minion +3 Attack. If it's a Demon, also give it +3 Health.
internal static class Sim_CFM_611
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
