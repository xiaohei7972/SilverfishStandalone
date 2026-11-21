using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 真言术：韧
// Power Word: Fortitude
// 使一个随从获得+3/+5。你手牌中每有一张法术牌，本牌的法力值消耗便减少（1）点。
// Give a minion +3/+5. Costs (1) less for each spell in your hand.
internal static class Sim_BAR_308
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
