using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 寒霜冰爆
// Frostshatter
// 冻结一个敌人。抽两张牌。（施放@个法术以变为随从牌！）
// Freeze an enemy. Draw 2 cards. (Cast @ |4(spell, spells) to turn into a minion!)
internal static class Sim_JAIL_803
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
