using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 青玉绽放
// Jade Blossom
// 召唤一个{0}的青玉魔像，获得一个空的法力水晶。@召唤一个青玉魔像，获得一个空的法力水晶。
// Summon a{1} {0} Jade Golem. Gain an empty Mana Crystal.@Summon a Jade Golem. Gain an empty Mana Crystal.
[CardSim("CFM_713")]
internal static class Sim_CFM_713
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_SLOT_OR_MANA_CRYSTAL_SLOT)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonJadeGolemEffect(), new ChangeManaEffect(ManaOperation.GainEmpty, 1)],
    };
}
