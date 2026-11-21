using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小型法术珍珠
// Lesser Pearl Spellstone
// 召唤一个2/2并具有嘲讽的灵魂。@（恢复3点生命值后升级。）
// Summon a 2/2 Spirit with Taunt. @(Restore 3 Health to upgrade.)
internal static class Sim_LOOT_091
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
