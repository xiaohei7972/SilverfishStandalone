using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 弗拉克的火箭炮
// Flark's Boom-Zooka
// 从你的牌库中召唤三个随从。他们会攻击敌方随从，然后死亡。
// Summon 3 minions from your deck. They attack enemy minions, then die.
internal static class Sim_BOT_429
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
