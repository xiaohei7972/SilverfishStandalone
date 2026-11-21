using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迪菲亚炸鱼手
// Defias Blastfisher
// 战吼： 随机对一个敌人造成2点伤害。每有一只你的野兽，重复一次。
// Battlecry: Deal 2 damage to a random enemy. Repeat for each of your Beasts.
internal static class Sim_DED_007
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
