using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 硬核信徒
// Hardcore Cultist
// 战吼：造成2点伤害。压轴：改为对所有敌人。
// Battlecry: Deal 2 damage. Finale: To all enemies.
internal static class Sim_ETC_209
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
