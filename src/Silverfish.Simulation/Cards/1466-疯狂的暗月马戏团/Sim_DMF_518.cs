using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 致伤打击
// Malevolent Strike
// 消灭一个随从。你的牌库中每有一张套牌之外的牌，本牌的法力值消耗便减少（1）点。
// Destroy a minion. Costs (1) less for each _ card in your deck that _ didn't start there.
internal static class Sim_DMF_518
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
