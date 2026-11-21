using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 摇滚堕落者
// Man'ari Mosher
// 战吼：在本回合中，使一个友方恶魔获得+3攻击力和吸血。
// Battlecry: Give a friendly Demon +3 Attack and Lifesteal this turn.
internal static class Sim_DMF_111
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DEMON)],
        []);
    // </legacy-play-requirements>

}
