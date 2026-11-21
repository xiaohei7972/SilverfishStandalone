using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 狱火邪谋
// Infernal Stratagem
// 使一个随从获得+3/+3。如果该随从是恶魔，你的下一张恶魔牌法力值消耗减少（2）点。
// Give a minion +3/+3. If it's a Demon, your next one costs (2) less.
internal static class Sim_GDB_122
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
