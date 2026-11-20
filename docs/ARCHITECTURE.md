# Architecture and boundaries

## Independent simulation engine

`SilverfishStandalone.sln` builds the independent simulator, its CLI, and its tests. The Windows x64 publish profile bundles the CLI with the .NET runtime and embedded starter data. The historical diagnostics build in a separate CI job.

`Silverfish.Domain.GameState` stores stable player and entity IDs. Each player owns ordered zone indexes. The entity table stores the card reference, owner, controller and character state. An inverse index provides zone lookup. A structural edit replaces the related indexes together. Changing control preserves the entity ID and entrance order.

The October 2026 client source provides a reference for identity, ownership and ordering. Its `GameState` looks up entities by ID, `Player` reaches its zones through `ZoneMgr`, and `Zone` maintains an ordered collection. The simulator keeps those relationships in immutable values so search branches can share unchanged state. Card resolution lives in `Silverfish.Simulation` and evaluation lives in `Silverfish.Core`.

`Silverfish.Simulation.GameSimulator` generates supported legal actions and resolves one action into a new immutable state and an ordered event trace. Card definitions compose typed effects. The existing classic snapshots and the classic engine provide historical fixtures for comparison.

`Silverfish.Core.SimulationTurnPlanner` searches branches up to a depth and transition budget. For each visited state, it generates and reuses one ordered set of legal actions for the outcome forecasts. The action set is bound to that immutable state. A separate scoring function evaluates each resulting state. The `plan` command runs the search after the fixture's scripted actions.

The `checkpoint` command serializes the state after an action finishes. Its versioned format carries the resolved card rules, ordered zones, entity state, death history, next entity and entry-order counters, random state, and match outcome. The simulator validates the stored counters, hero zones, and active stat layers on restore. A saved state can restart replay or planning and can supply the state in a decision request.

The replay command starts from a fully specified offline scenario. Its rules cover turn start and draw, fatigue, burning cards, basic minion and spell play, basic combat, control transfer, and deathrattle waves. Card definitions can subscribe to turn boundaries, summoning, spell resolution, and minion death. A queue records eligible listeners and runs nested events before returning to the parent listener. Minion-death listeners share the wave's eligibility snapshot and act in entry order alongside deathrattles. The normal single-token deathrattle uses the slot recorded when the minion was removed.

`samples/simulation-triggers.json` exercises spell and death timing through the normal replay command. Damage subscriptions can match the damaged entity or the source. Each fixed area or combat batch applies all hits before opening its damage queues. Armor loss produces a damage event, while a shield that absorbs the hit has a separate event. Each successful draw resolves the drawn card's intrinsic effects and other draw listeners before the next attempt. Fatigue dispatches damage, and a burned card never joins the draw queue.

`samples/simulation-damage-draw.json` selects the independently authored `starter` catalogue and replays Arcane Intellect, Acolyte of Pain, and Armorsmith together. The catalogue defines rules for 89 cards, powers, and tokens. Its printed stats, supported keywords, and Overload values come from an embedded subset of CardDefs build 256589. `CardDataCatalog` loads the versioned JSON and indexes it by card ID and DBF ID. The full export contains 23,452 entries, including card types and effects outside the current simulator's coverage. A scenario can use `cardRules` to associate explicit rules with card IDs in the selected export. The same metadata binding supplies the built-in rules and scenario rules.

Characters keep base attack and maximum health when modifiers first apply. The simulator applies direct enchantments in application order, then adds positional and friendly-board aura grants. Individual grants record their source entity. The simulator refreshes auras when a minion enters play and at resolution checkpoints, including after a death wave removes its minions. Silence clears direct enchantments and the minion's own abilities. Other minions can still provide it with aura grants. Maximum-health changes update current health without dispatching damage or healing events.

`samples/simulation-stats.json` replays Raid Leader and Bloodlust, then checks damage, turn-end expiration, and the remaining aura. An effect that sets Health clears earlier damage when applied. Later checkpoints preserve subsequent damage.

Weapons occupy the `Play` zone with their own entity IDs and durability. A player refers to its currently equipped weapon by ID. When a weapon is played, its battlecry runs before the equip reference changes. A swing applies both combat hits, resolves their damage events, then spends durability. Weapons and minions removed by the same death check resolve their deathrattles in entry order. Hero attack is derived from its own attack and the equipped weapon during its turn.

Hero powers occupy a dedicated single-entity zone. They pay mana and consume one use, refreshed when their controller's turn starts. `samples/simulation-equipment.json` replays Death's Bite, Dagger Mastery, and Fireblast.

Returning a minion clears its battlefield state and puts the same entity in its controller's hand. A full destination hand causes death and schedules deathrattles for the death phase. Hand cost changes record their source and expire when the card is played. Transformation sets a new card form on the same entity and retains its board position. A later death records the transformed card.

`minionDeaths` records the entity, card, controller, and turn at each death. It supplies resurrection candidates independently of the graveyard zone. A resurrection summons a fresh entity from the selected printed card. `samples/simulation-lifecycle.json` replays Shadowstep, Polymorph, and Resurrect.

Rush bypasses summoning sickness for attacks against minions. Windfury grants two attack slots and an equipped Windfury weapon uses the hero's existing attack count. A Frozen character still retaliates in combat. It thaws as its controller ends a turn in which it had an eligible attack slot, even if its Attack was zero. Silence clears Freeze. Direct Freeze effects can continue after damage is absorbed by a shield. Damage-triggered Freeze requires damage to be dealt.

Overload is charged during the use phase and stays pending until its controller next starts a turn. It then locks mana for that turn. An unlock effect restores the locked portion and clears pending Overload. `samples/simulation-combat-keywords.json` exercises Rush, a Windfury weapon, Freeze, and mana locked on the next turn.

Each player can arm up to five distinct secrets in the `Secret` zone. The supported secrets react on the opponent's turn and share entry ordering with battlefield listeners. Revealing a secret moves its entity to the graveyard. Counterspell runs before other spell-use triggers and Overload. Snipe runs after the played minion's battlecry. Effect summons skip the play-specific event.

Attack proposal listeners use the defender at the start of that proposal. Redirecting the attack starts another proposal after the current listeners finish. Freezing Trap rechecks whether the attacker can be returned. Explosive Trap can resolve after the attacker has already returned to hand. Combat needs a living attacker, defender, and both heroes after preparation. `samples/simulation-secrets.json` replays an armed Counterspell and both traps.

`randomState` seeds replay results. Single missiles rebuild their candidate pool after each damage event and its triggers. Cards such as Cleave draw distinct targets from one pool and apply their hits before running damage triggers. Random summons and destruction use the same choice mechanism as resurrection.

The planner enumerates weighted random paths within its resolution budget and samples when exact enumeration exceeds its allowance. It uses each outcome's available follow-up actions to estimate the current turn. `replanAfterLastAction` marks the point at which the caller needs to observe the real outcome. `usesSamples` marks a selected forecast that relies on sampling. The planner reports the win probability within its search horizon. The replay seed has no effect on planning probabilities. `samples/simulation-random.json` replays Animal Companion and plans an Arcane Missiles cast with an 87.5% lethal probability.

Players keep persistent crystals, spent mana, temporary mana, and overload separately. Available `mana` is derived from those counters and clamped at zero for payment. Temporary mana can exceed the permanent crystal cap, is spent first, and expires at the end of the owner's turn. Refresh fills only empty unlocked capacity and grants temporary mana. Older scenario files can specify `mana` and `manaCrystals` alone. To specify counters directly, supply `manaSpent` and any `temporaryMana` or overload values. The default permanent cap is ten and a fixture can override `maximumManaCrystals`.

Empty crystal gain preserves available mana, filled crystal gain can refill unused capacity at the cap, and configured ramp effects can generate Excess Mana at the permanent cap. Generated cards receive new entity IDs and enter the hand without a draw event. `samples/simulation-mana.json` replays The Coin, Nourish, Wild Growth, and Lightning Bloom.

Each play choice can declare its own target and play requirements. A legal action records the chosen option and any selected target. The simulator applies only that option's effects. `samples/simulation-choices.json` replays an untargeted summon and plans a targeted strike from the same card definition.

Conditional effects compare a living character's current stats or count entities in a player's zone. Each branch uses the same effect queue and preserves the parent effect context. Triggers evaluate their optional conditions immediately before activation. A secret whose condition fails stays armed. `samples/simulation-conditions.json` exercises a branch based on damage just dealt and a listener that checks the resulting armor.

The offline replay and planner take a fully specified scenario, including both players' secrets. Hidden information modelling, forced attacks, alternate weapon durability rules, extra hero power uses, hero replacement, cost auras, conditional auras, automatic spell consumption and replacement draws, and advanced summon placements remain future work.

## Components

    Observable offline GameSnapshot       Complete offline state + candidate actions
                      |                                      |
                SnapshotValidator                       GameSimulator
                      |                                      |
             BaselineStrategyEngine                SimulationDecisionEngine
                      |                                      |
                      +---------- Contracts.BotDecision -----+

The strategy engines operate on offline inputs. Their dependencies are the shared contracts and the independent simulator.

## Present milestone

The `decide` command accepts a versioned request containing a complete state and the calling application's candidate IDs. It verifies each candidate against the simulator's exact legal actions, including its position and choice. The turn planner limits its first move to that verified set. Future moves use the resulting simulation states. The response echoes the match ID, snapshot sequence, and the original first-move candidate. The observable snapshot command continues to use baseline scoring with its own input contract.

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
