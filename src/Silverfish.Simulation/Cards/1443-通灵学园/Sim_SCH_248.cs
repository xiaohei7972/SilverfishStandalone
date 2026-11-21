using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 甩笔侏儒
// Pen Flinger
// 战吼：对一个随从造成1点伤害。法术迸发：将本随从移回你的 手牌。
// Battlecry: Deal 1 damage to a minion. Spellburst: Return this to_your hand.
internal static class Sim_SCH_248
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
