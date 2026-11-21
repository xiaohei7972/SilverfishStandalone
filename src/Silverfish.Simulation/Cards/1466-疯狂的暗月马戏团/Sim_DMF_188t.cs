using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 腐蚀术
// Corruption
// 腐蚀你的所有卡牌。
// Corrupt all your cards.
internal static class Sim_DMF_188t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
