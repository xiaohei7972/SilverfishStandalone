using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 托瓦格尔的阴谋
// Togwaggle's Scheme
// 选择一个随从。将它的@张复制洗入你的牌库。（每回合都会升级！）
// Choose a minion. Shuffle @ |4(copy, copies) of it into your deck. (Upgrades each turn!)
internal static class Sim_DAL_010
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
