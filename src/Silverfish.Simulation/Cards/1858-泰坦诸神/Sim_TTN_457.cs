using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 悼词宣诵者
// Eulogizer
// 战吼：消耗3份残骸，造成3点伤害。锻造：改为获得残骸。
// Battlecry: Spend 3 Corpses to deal 3 damage. Forge: Gain them instead.
internal static class Sim_TTN_457
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
