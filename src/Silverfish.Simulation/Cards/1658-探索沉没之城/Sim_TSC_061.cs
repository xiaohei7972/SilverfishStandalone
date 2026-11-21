using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 花园惠景
// The Garden's Grace
// 使一个随从获得+4/+4和圣盾。在本局对战中，你每消耗1点法力值用于神圣法术牌上，本牌的法力值消耗便减少（1）点。
// Give a minion +4/+4 and Divine Shield. Costs (1) less for each Mana you've spent on Holy spells this game.
internal static class Sim_TSC_061
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
