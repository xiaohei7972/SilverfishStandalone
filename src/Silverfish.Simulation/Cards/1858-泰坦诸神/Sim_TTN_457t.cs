using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 悼词宣诵者
// Eulogizer
// 已锻造 战吼：获得3份残骸。造成3点伤害。
// Forged Battlecry: Gain 3 Corpses. Deal 3 damage.
internal static class Sim_TTN_457t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
