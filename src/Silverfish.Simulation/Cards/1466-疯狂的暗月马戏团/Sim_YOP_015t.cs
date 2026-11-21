using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 氮素药膏
// Nitroboost Poison
// 已腐蚀 使一个随从和你的武器获得+2攻击力。
// Corrupted Give a minion and your weapon +2 Attack.
internal static class Sim_YOP_015t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
