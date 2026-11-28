using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 桌游角色扮演玩家
// Tabletop Roleplayer
// 微缩 战吼：在本回合中，使一个友方恶魔获得+2攻击力和免疫。
// Miniaturize Battlecry: Give a friendly Demon +2 Attack and Immune this turn.
internal static class Sim_TOY_915
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DEMON), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
