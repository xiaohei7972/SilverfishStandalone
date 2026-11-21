using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰霜巨龙之怒
// Frostwyrm's Fury
// 造成$5点伤害。冻结所有敌方随从。召唤一条5/5的冰霜巨龙。
// Deal $5 damage. Freeze all enemy minions. Summon a 5/5 Frostwyrm.
internal static class Sim_RLK_063
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
