using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大地之鳞
// Earthen Scales
// 使一个友方随从获得+1/+1，然后获得等同于其攻击力的 护甲值。
// Give a friendly minion +1/+1, then gain Armor equal to its Attack.
internal static class Sim_CORE_UNG_108
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
