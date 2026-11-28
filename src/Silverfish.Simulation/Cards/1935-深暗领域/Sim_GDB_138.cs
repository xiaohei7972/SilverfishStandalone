using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神性圣契
// Libram of Divinity
// 使一个随从获得+3/+3。如果本牌的法力值消耗为（0）点，在你的回合结束时将本牌移回你的手牌。
// Give a minion +3/+3. If this costs (0), return this to your hand at the end of your turn.
internal static class Sim_GDB_138
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
