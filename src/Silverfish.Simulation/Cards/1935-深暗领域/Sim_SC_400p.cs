using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 强化剂
// Stimpack
// 召唤一个2/2并具有 嘲讽的陆战队员。使你的人族随从获得+2攻击力。
// Summon a 2/2 Marine with Taunt. Give your Terran minions +2_Attack.
internal static class Sim_SC_400p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
