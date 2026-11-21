using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 即兴演奏
// Jam Session
// 使一个友方随从获得+3/+3。对所有其他随从造成$1点伤害。过载：（1）
// Give a friendly minion +3/+3. Deal $1 damage to all other minions. Overload: (1)
internal static class Sim_JAM_013
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
