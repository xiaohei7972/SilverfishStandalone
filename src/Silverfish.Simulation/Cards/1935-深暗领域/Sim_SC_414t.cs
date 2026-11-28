using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雷神（爆裂弹头）
// Thor, Explosive Payload
// 战吼：造成5点伤害。 在本局对战中你每发射过一艘星舰，对一个随机敌人重复一次。@（重复@次）
// Battlecry: Deal 5 damage. __Repeat at a random enemy__ for each Starship you've launched this game.@ (@)
internal static class Sim_SC_414t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
