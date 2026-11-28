using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 折纸巨龙
// Origami Dragon
// 圣盾，吸血 战吼：与另一个随从交换属性值。
// Divine Shield, Lifesteal Battlecry: Swap stats with another minion.
internal static class Sim_TOY_896
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
