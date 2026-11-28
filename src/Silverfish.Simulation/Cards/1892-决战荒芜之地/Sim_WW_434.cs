using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 日斑巨龙
// Sunspot Dragon
// 可交易 吸血。快枪：造成6点伤害。
// Tradeable, Lifesteal Quickdraw: Deal 6 damage.
internal static class Sim_WW_434
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_NONSELF_TARGET)],
        []);
    // </legacy-play-requirements>

}
