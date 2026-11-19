# Architecture and boundaries

## Independent simulation engine

`Silverfish.Domain.GameState` stores stable player and entity IDs. Each player owns ordered zone indexes. The entity table stores the card reference, owner, controller and character state. An inverse index provides zone lookup. A structural edit replaces the related indexes together. Changing control preserves the entity ID and entrance order.

The October 2026 client source provides a reference for identity, ownership and ordering. Its `GameState` looks up entities by ID, `Player` reaches its zones through `ZoneMgr`, and `Zone` maintains an ordered collection. The simulator keeps those relationships in immutable values so search branches can share unchanged state. Card resolution lives in `Silverfish.Simulation` and evaluation lives in `Silverfish.Core`.

`Silverfish.Simulation.GameSimulator` generates supported legal actions and resolves one action into a new immutable state and an ordered event trace. Card definitions compose typed effects. The existing classic snapshots and the classic engine provide historical fixtures for comparison.

`Silverfish.Core.SimulationTurnPlanner` searches branches up to a depth and transition budget. It applies each candidate through the new simulator and evaluates the resulting state through a separate scoring function. The `plan` command runs it after the fixture's scripted actions.

The replay command starts from a fully specified offline scenario. Its rules cover turn start and draw, fatigue, burning cards, basic minion and spell play, basic combat, control transfer, and deathrattle waves. Card definitions can subscribe to turn boundaries, summoning, spell resolution, and minion death. A queue records eligible listeners and runs nested events before returning to the parent listener. Minion-death listeners share the wave's eligibility snapshot and act in entry order alongside deathrattles. The normal single-token deathrattle uses the slot recorded when the minion was removed.

`samples/simulation-triggers.json` exercises spell and death timing through the normal replay command. Damage subscriptions can match the damaged entity or the source. Each fixed area or combat batch applies all hits before opening its damage queues. Armor loss produces a damage event, while a shield that absorbs the hit has a separate event. Each successful draw resolves the drawn card's intrinsic effects and other draw listeners before the next attempt. Fatigue dispatches damage, and a burned card never joins the draw queue.

`samples/simulation-damage-draw.json` selects the independently authored `starter` catalogue and replays Arcane Intellect, Acolyte of Pain, and Armorsmith together. The catalogue defines 29 cards, powers, and tokens using the November 2026 data baseline.

Characters keep base attack and maximum health when modifiers first apply. The simulator applies direct enchantments in application order, then adds positional and friendly-board aura grants. Individual grants record their source entity. The simulator refreshes auras when a minion enters play and at resolution checkpoints, including after a death wave removes its minions. Silence clears direct enchantments and the minion's own abilities. Other minions can still provide it with aura grants. Maximum-health changes update current health without dispatching damage or healing events.

`samples/simulation-stats.json` replays Raid Leader and Bloodlust, then checks damage, turn-end expiration, and the remaining aura. An effect that sets Health clears earlier damage when applied. Later checkpoints preserve subsequent damage.

Weapons occupy the `Play` zone with their own entity IDs and durability. A player refers to its currently equipped weapon by ID. When a weapon is played, its battlecry runs before the equip reference changes. A swing applies both combat hits, resolves their damage events, then spends durability. Weapons and minions removed by the same death check resolve their deathrattles in entry order. Hero attack is derived from its own attack and the equipped weapon during its turn.

Hero powers occupy a dedicated single-entity zone. They pay mana and consume one use, refreshed when their controller's turn starts. `samples/simulation-equipment.json` replays Death's Bite, Dagger Mastery, and Fireblast. Alternative weapon durability rules, extra hero power uses, hero replacement, cost modifiers, conditional auras, automatic spell consumption and replacement draws, advanced summon placements, and hidden information modelling remain future work.

## Components

    Offline JSON or StandaloneGameMod state.snapshot
                      |
                      v
       Snapshot adapter (not yet implemented)
                      |
                      v
         Contracts.GameSnapshot (v1)
                      |
             SnapshotValidator
                      |
                      v
          IStrategyEngine.DecideAsync
             /                    \
    BaselineStrategyEngine    Future SilverfishPort
             \                    /
                      v
              Contracts.BotDecision
                      |
              CLI or future UI

The strategy interface has no reference to Libmem.NET, Unity, Mono, Hearthbuddy, Triton or the UI. The source and the algorithm can evolve separately.

## Present milestone

The CLI consumes offline JSON fixtures with explicitly enumerated candidate actions. BaselineStrategyEngine ranks them deterministically. This is a working protocol/proof-of-concept, not Silverfish's original game simulator.

The offline contract is not identical to CardResearchLab/StandaloneGameMod's state.snapshot payload. A separate, tested versioned translation layer is required before consuming its live data. Do not deserialize state.snapshot directly as Contracts.GameSnapshot.

The optional Libmem.NET project uses 2.5.0 and explicit Windows x64. At present, it only probes metadata of a PID selected by the caller; it does not reconstruct gameplay entities. Its existence must not be confused with a working game-state reader.

## Non-negotiable runtime invariants

1. Input data must be labeled with schema version, match ID and monotonic sequence.
2. Invalid or unsupported snapshots must fail closed, not silently generate guesses.
3. UI and strategy components never hold process handles.
4. Never infer an action is legal from baseline heuristics. Action candidates must originate from a trusted, separately verified provider.
5. Reject decisions if their match ID or sequence no longer matches the latest snapshot.
6. No gameplay execution is implemented here. Any future executor needs separate authorization, state revalidation, and an offline test harness.
7. Sensitive tokens and secrets must never enter game snapshots, logs, or the UI's decision telemetry.

## Integration design

The existing StandaloneGameMod has an authenticated localhost TCP envelope (protocolVersion=2) with state.snapshot event messages. A consumer should re-use that authenticated transport, select the correct process instance and session, verify sequence continuity, map game state into the model and record regression fixtures. Do not add game-specific or bot-specific dependencies to Libmem.NET.

The new simulator needs an adapter from verified snapshot data to its players, ordered zones, and entities. Historical replay through ClassicBridge has its own adapter for the Hrtprozis and Playfield model. Strategy code receives immutable simulation state and candidate IDs from the applicable adapter.
