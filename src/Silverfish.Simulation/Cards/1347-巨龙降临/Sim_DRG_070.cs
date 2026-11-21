using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 幼龙饲养员
// Dragon Breeder
// 战吼：选择一条友方的龙。将它的一张复制置入你的手牌。
// Battlecry: Choose a friendly Dragon. Add a copy of it to_your hand.
internal static class Sim_DRG_070
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DRAGON), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
