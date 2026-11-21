using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 炮灰出动
// Wreck'em and Deck'em
// 选择一个友方机械，召唤一个它的复制并使其攻击随机敌人然后死亡。
// Choose a friendly Mech. Summon a copy of it that attacks a random enemy, then dies.
internal static class Sim_TOY_603
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.MECHANICAL)],
        []);
    // </legacy-play-requirements>

}
