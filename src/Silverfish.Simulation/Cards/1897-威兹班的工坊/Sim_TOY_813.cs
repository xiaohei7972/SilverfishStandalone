using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 玩具队长塔林姆
// Toy Captain Tarim
// 微缩 嘲讽。战吼：将一个随从的攻击力和生命值变为与本随从相同。
// Miniaturize Taunt. Battlecry: Set a minion's Attack and Health to this minion's.
internal static class Sim_TOY_813
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
