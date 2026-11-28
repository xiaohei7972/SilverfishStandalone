using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 玛润
// Grunty
// 战吼：随机召唤四个鱼人，然后将其射向敌方随从。（目标由你选定！）
// Battlecry: Summon four random Murlocs, then shoot them at enemy minions. _(You pick the targets!)
internal static class Sim_SC_013
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
