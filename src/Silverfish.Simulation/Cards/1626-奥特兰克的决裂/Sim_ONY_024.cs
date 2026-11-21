using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奥妮克希亚幼龙
// Onyxian Drake
// 嘲讽，战吼：对一个敌方随从造成等同于你的护甲值的伤害。
// Taunt Battlecry: Deal damage equal to your Armor to an enemy minion.
internal static class Sim_ONY_024
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
