namespace Silverfish.Simulation.Cards;

// 小个子驱魔者
// Lil' Exorcist
// 嘲讽，战吼：每有一个具有亡语的敌方随从，便获得+1/+1。
// Taunt Battlecry: Gain +1/+1 for each enemy Deathrattle minion.
[CardSim("GVG_097")]
internal static class Sim_GVG_097
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ScaledEnchantStatsEffect(StatTarget.Source, StatScaleSource.EnemyDeathrattleMinions, 1, 1)]
    };
}
