using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 燃烧权杖
// Rod of Roasting
// 随机施放炎爆术直到一方英雄 死亡。
// Cast 'Pyroblast' randomly until a hero dies.
internal static class Sim_DMF_004t6
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
