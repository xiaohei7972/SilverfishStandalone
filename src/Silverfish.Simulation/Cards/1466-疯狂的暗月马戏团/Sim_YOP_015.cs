using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 氮素药膏
// Nitroboost Poison
// 使一个随从获得+2攻击力。腐蚀：并使你的武器获得+2攻击力。
// Give a minion +2 Attack. Corrupt: And your weapon.
internal static class Sim_YOP_015
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
