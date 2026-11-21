using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 狗狗饼干
// Doggie Biscuit
// 可交易 使一个随从获得+2/+3。在你交易此牌后，使一个友方随从获得突袭。
// Tradeable Give a minion +2/+3. After you Trade this, give a friendly minion Rush.
internal static class Sim_DED_009
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
