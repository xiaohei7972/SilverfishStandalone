using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 阴燃电鳗
// Murkspark Eel
// 战吼： 如果你的牌库中只有法力值消耗为偶数的牌，造成2点伤害。
// Battlecry: If your deck has only even-Cost cards, deal_2 damage.
internal static class Sim_GIL_530
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_NONSELF_TARGET)],
        []);
    // </legacy-play-requirements>

}
