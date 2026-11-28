'use strict';

const byId = id => document.getElementById(id);
const number = value => value ?? '未知';
const score = value => value == null ? '—' : value.toLocaleString('zh-CN', { maximumFractionDigits: 2 });
const phases = { unknown: '阶段未知', mulligan: '起手换牌', main: '主要阶段', choice: '等待选择', gameOver: '对局结束' };
const actionNames = { endTurn: '结束回合', attack: '攻击', playCard: '打出', heroPower: '使用英雄技能',
  useHeroPower: '使用英雄技能', activateLocation: '激活地标', chooseOption: '提交选择',
  choose_entity: '选择卡牌', confirm_choice: '确认选择', end_turn: '结束回合' };
const sampleNames = {
  'classic-snapshot-lethal.json': '经典局面：攻击斩杀', 'lethal.json': '可观察局面：基础斩杀',
  'opponent-turn.json': '可观察局面：敌方回合', 'standalone-state-snapshot.json': '客户端快照：公开牌面',
  'simulation-card-data.json': '卡牌目录：训练武器', 'simulation-choices.json': '出牌抉择',
  'simulation-combat-keywords.json': '战斗关键词', 'simulation-combat.json': '基础战斗',
  'simulation-conditions.json': '条件结算', 'simulation-cost-auras.json': '费用光环',
  'simulation-damage-draw.json': '伤害与抽牌', 'simulation-equipment.json': '武器与装备',
  'simulation-lifecycle.json': '区域与实体变化', 'simulation-mana.json': '法力水晶',
  'simulation-pending-choice.json': '结算中的待决选择', 'simulation-random.json': '随机结果与规划',
  'simulation-secrets.json': '奥秘结算', 'simulation-stats.json': '属性与附魔', 'simulation-triggers.json': '触发与亡语'
};
const eventNames = {
  turnEnded: '回合结束', turnStarted: '回合开始', cardDrawn: '抽牌', cardBurned: '爆牌', fatigue: '疲劳',
  cardPlayed: '打出卡牌', summoned: '召唤', controlChanged: '控制权变更', attack: '攻击', damage: '造成伤害',
  shieldBroken: '圣盾破裂', died: '死亡', deathrattle: '结算亡语', matchEnded: '对局结束', triggered: '触发效果',
  armorGained: '获得护甲', drawnEffect: '触发抽牌效果', enchantmentApplied: '施加附魔', enchantmentExpired: '附魔消失',
  silenced: '沉默', weaponEquipped: '装备武器', weaponDurabilityLost: '损失武器耐久', weaponDestroyed: '武器摧毁',
  heroPowerUsed: '使用英雄技能', healed: '恢复生命', returnedToHand: '移回手牌', transformed: '变形',
  resurrected: '复活', frozen: '冻结', thawed: '解冻', overloadScheduled: '记录过载', overloadUnlocked: '解除过载',
  secretArmed: '挂起奥秘', secretRevealed: '揭示奥秘', spellCountered: '反制法术', spellRedirected: '重定向法术',
  attackRedirected: '重定向攻击', attackCancelled: '取消攻击', temporaryManaGained: '获得临时法力',
  manaCrystalsGained: '获得法力水晶', manaCrystalsDestroyed: '摧毁法力水晶', manaRefreshed: '恢复法力',
  cardGenerated: '生成卡牌', locationActivated: '激活地标', locationHealthLost: '地标减少次数', locationDestroyed: '地标摧毁',
  cardDiscarded: '弃牌', discardedEffect: '触发弃牌效果', recruited: '招募', spellCastWhenDrawn: '抽到时施放',
  cardAddedToDeck: '将牌加入牌库', deckShuffled: '洗牌', alternateCostPaid: '支付替代费用', costTypeChanged: '费用类型变更',
  playChoiceOffered: '提出抉择', playChoiceSelected: '选定抉择', choiceSubmitted: '提交选择', overhealed: '过量治疗'
};

let current = null;
let binding = null;
let mutation = false;
let pollVersion = 0;
let stateKey = '';
let bindingKey = '';
let stateEtag = '';
let entityById = new Map();
let selectedEntityId = null;
let entityLimit = 100;
let entityListKey = '';

function element(tag, text, className) {
  const node = document.createElement(tag);
  if (text != null) node.textContent = text;
  if (className) node.className = className;
  return node;
}

async function api(path, body) {
  const options = body === undefined ? {} : { method: 'POST',
    headers: { 'Content-Type': 'application/json', 'X-Silverfish-Console': '1' }, body };
  return (await request(path, options)).json();
}

async function request(path, options) {
  let response;
  try { response = await fetch(`/api/${path}`, options); }
  catch { throw new Error('无法访问本地控制台，请检查命令行服务是否仍在运行。'); }
  if (!response.ok && response.status !== 304) {
    const data = await response.json().catch(() => null);
    throw new Error(data?.error || `请求失败（${response.status}）。`);
  }
  return response;
}

function notice(message, error = false, target = 'notice') {
  const node = byId(target);
  node.textContent = message || '';
  node.classList.toggle('error', error);
  node.hidden = !message;
}

function paintControls() {
  byId('load-sample').disabled = mutation || !byId('samples').value;
  byId('reset').disabled = mutation || (!current?.board && !current?.automatic && !current?.autoPilot);
  byId('decide').disabled = mutation || !current?.board || current.busy;
  byId('automatic').disabled = mutation;
  byId('autopilot').disabled = mutation || !current;
  byId('refresh-clients').disabled = mutation;
  byId('inject-client').disabled = mutation || !binding || binding.status !== 'online';
  byId('disconnect').disabled = mutation || !binding;
  byId('import-submit').disabled = mutation;
  document.querySelectorAll('[data-operation]').forEach(button => { button.disabled = mutation; });
}

async function operate(task, target = 'notice') {
  if (mutation) return;
  pollVersion++;
  mutation = true;
  paintControls();
  notice('', false, target);
  try { await task(); }
  catch (error) { notice(error.message, true, target); }
  finally { mutation = false; paintControls(); }
}

function change(route, payload, message) {
  return operate(async () => {
    showState(await api(route, JSON.stringify(payload)));
    if (message) notice(message);
  });
}

function heroRow(id, hero, side, counts) {
  const row = byId(id);
  row.replaceChildren();
  if (!hero) return;
  row.append(element('strong', `${side}英雄 · ${hero.className || '职业未知'}`), element('span', `生命 ${hero.health}`, 'stat hp'),
    element('span', `护甲 ${hero.armor}`, 'stat'), element('span', `攻击 ${hero.attack}`, 'stat'));
  for (const [label, value] of counts) row.append(element('span', `${label} ${number(value)}`, 'muted'));
}

function cardsAt(id, cards, empty) {
  const row = byId(id);
  row.replaceChildren();
  if (!cards?.length) { row.append(element('p', empty, 'empty')); return; }
  for (const card of cards) {
    const canInspect = entityById.has(card.entityId);
    const node = element(canInspect ? 'button' : 'article', null, canInspect ? 'card inspectable-card' : 'card');
    if (canInspect) {
      node.setAttribute('aria-controls', 'entity-detail');
      node.title = '查看实体 Tag';
      node.addEventListener('click', () => selectEntity(card.entityId, true));
    }
    node.append(element('strong', card.name || card.cardId), element('small', `${card.cardId} / #${card.entityId}`));
    const stats = element('span', null, 'stats');
    if (card.attack != null) stats.append(element('span', `攻 ${card.attack}`, 'stat'));
    if (card.health != null) stats.append(element('span', `${card.statuses?.includes('地标') ? '次数' : '生命'} ${card.health}`, 'stat hp'));
    if (card.cost != null) stats.append(element('span', `费用 ${card.cost}`, 'stat mana'));
    node.append(stats);
    if (card.statuses?.length) node.append(element('span', card.statuses.join(' / '), 'keywords'));
    row.append(node);
  }
}

function updateEntities(board, reset) {
  const search = byId('entity-search');
  const zones = byId('entity-zone');
  if (reset) {
    selectedEntityId = null;
    search.value = '';
    zones.value = '';
    entityLimit = 100;
  }
  entityById = new Map((board?.allEntities ?? []).map(entity => [entity.id, entity]));
  if (!entityById.has(selectedEntityId)) selectedEntityId = null;
  search.disabled = zones.disabled = !board?.allEntities?.length;
  const zoneNames = [...new Set([...entityById.values()].map(entity => entity.zone).filter(Boolean))].sort();
  if (JSON.stringify(zoneNames) !== JSON.stringify([...zones.options].slice(1).map(option => option.value))) {
    const previous = zones.value;
    zones.length = 1;
    for (const name of zoneNames) zones.add(new Option(name, name));
    zones.value = zoneNames.includes(previous) ? previous : '';
  }
  renderEntityList();
  renderEntityDetail();
}

function renderEntityList() {
  const node = byId('entity-list');
  const search = byId('entity-search').value.trim().toUpperCase();
  const idSearch = search.replace(/^#(?=\d)/, '');
  const zone = byId('entity-zone').value;
  const results = [...entityById.values()].filter(entity => (!zone || entity.zone === zone) &&
    (!search || String(entity.id).includes(idSearch) || entity.cardId?.toUpperCase().includes(search)));
  const shown = results.slice(0, entityLimit);
  byId('entity-count').textContent = entityById.size ? `${results.length} / ${entityById.size} 个实体` : '';
  byId('entity-more').hidden = shown.length >= results.length;
  const key = JSON.stringify([entityById.size > 0, shown.map(entity => [entity.id, entity.cardId, entity.cardType, entity.zone, entity.isRedacted])]);
  if (key !== entityListKey) {
    entityListKey = key;
    const focusedId = document.activeElement?.dataset.entityId;
    node.replaceChildren();
    for (const entity of shown) {
      const row = element('li');
      const button = element('button', null, 'entity-row');
      button.dataset.entityId = entity.id;
      button.setAttribute('aria-controls', 'entity-detail');
      button.addEventListener('click', () => selectEntity(entity.id));
      const identity = entity.isRedacted ? '已隐去' : entity.cardId || entity.cardType || '—';
      button.append(element('strong', `#${entity.id} · ${identity}`), element('small', entity.zone || '未知区域', 'muted'));
      row.append(button);
      node.append(row);
    }
    if (!shown.length) node.append(element('li', entityById.size ? '没有符合筛选的实体。' : '载入客户端快照后显示实体。', 'empty'));
    if (focusedId) node.querySelector(`[data-entity-id="${focusedId}"]`)?.focus({ preventScroll: true });
  }
  for (const button of node.querySelectorAll('button'))
    button.setAttribute('aria-pressed', Number(button.dataset.entityId) === selectedEntityId);
}

function selectEntity(id, fromCard = false) {
  selectedEntityId = id;
  renderEntityList();
  renderEntityDetail();
  if (fromCard) {
    byId('entity-inspector').scrollIntoView({ block: 'nearest' });
    byId('entity-detail').focus({ preventScroll: true });
  }
}

function renderEntityDetail() {
  const node = byId('entity-detail');
  const entity = entityById.get(selectedEntityId);
  node.replaceChildren();
  if (!entity) { node.append(element('p', '选择实体查看 Tag。', 'empty')); return; }
  node.append(element('h3', `实体 #${entity.id}${entity.cardId ? ` · ${entity.cardId}` : ''}`),
    element('p', [entity.cardType, entity.zone, `控制者 ${entity.controllerId || '未知'}`].filter(Boolean).join(' · '), 'muted'));
  if (entity.isRedacted) { node.append(element('p', '未公开区域：卡牌身份及 Tag 已隐去。', 'empty')); return; }
  appendTags(node, 'Tag', entity.tags);
  if (entity.cachedTagsForDormant?.length) appendTags(node, '休眠缓存 Tag', entity.cachedTagsForDormant);
}

function appendTags(node, title, tags) {
  if (!tags?.length) { node.append(element('p', '快照未提供 Tag。', 'empty')); return; }
  const wrapper = element('div', null, 'tag-table-wrap scrollbox');
  const table = element('table', null, 'tag-table');
  const header = element('tr');
  for (const label of ['ID', '名称', '值']) header.append(element('th', label));
  const head = element('thead');
  head.append(header);
  const body = element('tbody');
  for (const tag of [...tags].sort((left, right) => left.id - right.id)) {
    const row = element('tr');
    row.append(element('td', tag.id), element('td', tag.name), element('td', tag.value));
    body.append(row);
  }
  table.append(element('caption', title), head, body);
  wrapper.append(table);
  node.append(wrapper);
}

async function sendLiveCommand(kind, sourceEntityId = null, targetEntityId = null, label = '') {
  return operate(async () => {
    await api('command', JSON.stringify({ kind, sourceEntityId, targetEntityId }));
    notice(`已下发游戏内虚拟操作：${label || kind}（不抢占系统鼠标）`);
  });
}

function renderPendingChoice(board) {
  const panel = byId('choice-panel');
  const cardsNode = byId('choice-cards');
  const metaNode = byId('choice-meta');
  const confirmBtn = byId('confirm-choice');
  const choice = board?.pendingChoice;
  if (!choice?.cards?.length) {
    panel.hidden = true;
    cardsNode.replaceChildren();
    confirmBtn.hidden = true;
    return;
  }
  panel.hidden = false;
  const modeText = choice.isSingleChoice ? '单选（点击卡牌直接确认）' : `可选 ${choice.countMin} ~ ${choice.countMax} 项`;
  metaNode.textContent = `选择包 #${choice.choiceId} · ${choice.choiceType} · ${modeText}`;
  confirmBtn.hidden = !choice.hasConfirmButton;
  cardsNode.replaceChildren();
  for (const card of choice.cards) {
    const box = element('article', null, `card choice-card${card.chosen ? ' chosen-card' : ''}`);
    box.append(element('strong', card.name || card.cardId), element('small', `${card.cardId} / #${card.entityId}`));
    const stats = element('span', null, 'stats');
    stats.append(element('span', `费用 ${card.cost}`, 'stat mana'));
    if (card.attack != null) stats.append(element('span', `攻 ${card.attack}`, 'stat'));
    if (card.health != null) stats.append(element('span', `血 ${card.health}`, 'stat hp'));
    box.append(stats);
    const btn = element('button', card.chosen ? '已选（再次点击取消）' : '选择此项', card.chosen ? '' : 'primary');
    btn.disabled = card.unchoosable || mutation;
    btn.dataset.operation = 'live-choice';
    btn.addEventListener('click', () => sendLiveCommand('choose_entity', card.entityId, null, `选择 ${card.name || card.cardId} (#${card.entityId})`));
    box.append(btn);
    cardsNode.append(box);
  }
}

function setActionGateHint(button, board) {
  const blocked = board?.canAct === false;
  button.title = blocked ? '等待游戏就绪，托管不会发令；仍可手动操作。' : '';
  if (blocked) button.setAttribute('aria-describedby', 'action-gate');
  else button.removeAttribute('aria-describedby');
}

function renderBoard(state) {
  const board = state.board;
  byId('snapshot-mode').textContent = state.mode === 'simulation' ? '离线模拟' : board ? '实时/可观察快照' : '未载入';
  const meta = byId('match-meta');
  meta.replaceChildren();
  meta.hidden = !board;
  byId('turn-indicator').textContent = !board ? '等待载入局面' : board.phase === 'gameOver' || board.phase === 'mulligan'
    ? phases[board.phase] : board.isOurTurn ? '我方回合' : '敌方回合';
  const endTurnBtn = byId('live-end-turn');
  endTurnBtn.hidden = !(state.mode === 'snapshot' && board?.isOurTurn && !board?.pendingChoice);
  setActionGateHint(endTurnBtn, board);
  const actionGate = byId('action-gate');
  const blocked = board?.canAct === false;
  actionGate.hidden = !board || state.mode !== 'snapshot';
  actionGate.dataset.ready = String(!blocked);
  actionGate.textContent = actionGate.hidden ? '' : blocked
    ? `等待游戏就绪：${board.blockReason?.trim() || '客户端暂不可操作'}` : '可操作';
  renderPendingChoice(board);
  if (board) {
    const pairs = [['对局 ID', state.matchId], ['序号', state.sequence], ['阶段', phases[board.phase] || '未知'],
      ['我方法力', `${board.availableMana} / ${number(board.maximumMana)}`]];
    if (board.turn != null) pairs.push(['回合', board.turn]);
    if (board.responseMode?.trim()) pairs.push(['响应模式', board.responseMode]);
    for (const [key, value] of pairs) {
      const item = element('div');
      item.append(element('dt', key), element('dd', value));
      meta.append(item);
    }
  }
  heroRow('enemy-hero', board?.enemyHero, '敌方', [['手牌', board?.enemyHandCount], ['牌库', board?.enemyDeckCount], ['奥秘', board?.enemySecretCount]]);
  heroRow('our-hero', board?.ourHero, '我方', [['奥秘', board?.ourSecretCount]]);
  cardsAt('enemy-board', board?.enemyBoard, board ? '敌方战场为空' : '敌方战场');
  cardsAt('our-board', board?.ourBoard, board ? '我方战场为空' : '我方战场');
  cardsAt('our-hand', board?.ourHand, board ? '没有手牌。' : '载入局面后显示手牌。');
  byId('hand-count').textContent = board ? `${board.ourHand.length} 张` : '';
}

function entityNames(board) {
  const names = new Map();
  if (!board) return names;
  names.set(board.ourHero.entityId, '我方英雄');
  names.set(board.enemyHero.entityId, '敌方英雄');
  for (const card of [...board.ourBoard, ...board.enemyBoard, ...board.ourHand]) names.set(card.entityId, card.name || card.cardId);
  for (const card of board.pendingChoice?.cards ?? []) names.set(card.entityId, card.name || card.cardId);
  return names;
}

function actionLabel(action, names) {
  if (!action) return '暂无推荐动作';
  const source = action.sourceEntityId ?? action.source?.value;
  const target = action.targetEntityId ?? action.target?.value;
  const entity = id => names.has(id) ? `${names.get(id)} #${id}` : `实体 #${id}`;
  const name = actionNames[action.kind] || '未识别动作';
  return [name, source != null ? entity(source) : '', target != null ? `目标：${entity(target)}` : '',
    action.position != null ? `位置 ${action.position + 1}` : '', action.choice ? `选项：${names.get(action.choice) || action.choice}` : '',
    action.orderedChoiceTargets?.length ? `依次选择：${action.orderedChoiceTargets.map(id => entity(id.value)).join('、')}` : '',
    action.choicePacketId != null ? `选择 #${action.choicePacketId}` : ''].filter(Boolean).join(' · ');
}

function renderDecision(state, names) {
  const autopilot = byId('autopilot');
  autopilot.textContent = state.autoPilot ? '停止银鱼托管' : '开启银鱼托管';
  autopilot.setAttribute('aria-pressed', state.autoPilot);
  const pilotStatus = byId('autopilot-status');
  pilotStatus.textContent = state.autoPilot ? '全自动托管中' : '托管已关闭';
  pilotStatus.dataset.live = state.autoPilot;
  byId('autopilot-controls').dataset.live = state.autoPilot;

  const ack = state.lastCommandAck;
  const receipt = byId('command-ack');
  const action = state.lastCommandAction;
  const label = action && action.id === ack?.id ? `${actionLabel(action, new Map())} · ` : '';
  const receiptText = ack ? `${ack.ok ? '执行成功' : '执行失败'} · ${label}指令 #${ack.id}：${ack.message}` : '尚未收到执行回执。';
  if (receipt.textContent !== receiptText) receipt.textContent = receiptText;
  receipt.classList.toggle('muted', !ack);
  receipt.classList.toggle('error', ack?.ok === false);
  receipt.classList.toggle('success', ack?.ok === true);

  const automatic = byId('automatic');
  automatic.textContent = state.automatic ? '暂停自动（仅分析）' : '启动自动（仅分析）';
  automatic.setAttribute('aria-pressed', state.automatic);
  const status = byId('automatic-status');
  status.textContent = state.busy ? '计算中' : state.automatic ? '自动轮询' : '已暂停';
  status.dataset.live = state.automatic;
  const node = byId('decision');
  node.replaceChildren();
  const plan = state.decision?.plan;
  const baseline = state.decision?.baseline;
  if (plan) {
    node.append(element('strong', actionLabel(plan.actions[0], names)), element('p',
      `期望评分 ${score(plan.score)} / 搜索胜率 ${score(plan.winProbability * 100)}%`, 'stat mana'),
      element('p', `${state.decision.reason} 结算 ${plan.transitions} 次${plan.usesSamples ? '，含随机抽样' : ''}。`, 'muted'));
    if (plan.actions.length) {
      const steps = element('ol');
      for (const step of plan.actions) steps.append(element('li', actionLabel(step, names)));
      node.append(steps);
      if (plan.replanAfterLastAction) node.append(element('p', '规划到此为止；请按实际随机结果重新规划。', 'muted'));
    }
  } else if (baseline) {
    node.append(element('strong', actionLabel(baseline.chosenAction, names)), element('p', baseline.reason, 'muted'));
    if (baseline.score != null) node.append(element('p', `基线评分 ${score(baseline.score)}`, 'stat mana'));
  } else {
    node.append(element('p', state.decisionError || state.decision?.reason || (state.board
      ? '使用单步决策，或启动自动分析。' : '载入局面后可生成建议。单步决策和自动分析不会下发操作。'),
    state.decisionError ? 'error' : 'empty'));
  }

  const actions = state.legalActions ?? state.observedActions ?? [];
  byId('actions-heading').textContent = state.mode === 'snapshot' ? '快照提供的候选' : '合法动作';
  byId('action-count').textContent = state.board ? `${actions.length} 项` : '';
  const list = byId('actions');
  const scroll = list.scrollTop;
  list.replaceChildren();
  if (!actions.length) list.append(element('li', '暂无动作。', 'empty'));
  const recommendation = plan?.actions[0] ? JSON.stringify(plan.actions[0]) : null;
  actions.forEach((action, index) => {
    const selected = recommendation ? JSON.stringify(action) === recommendation : baseline?.chosenAction?.id === action.id;
    const row = element('li', null, selected ? 'recommended' : null);
    const label = actionLabel(action, names);
    row.append(element('span', label));
    if (state.mode === 'simulation') {
      const apply = element('button', '推演');
      apply.dataset.operation = 'apply';
      apply.setAttribute('aria-label', `推演：${label}`);
      apply.addEventListener('click', () => change('apply', { version: state.version, index }, '已推演一个动作。'));
      row.append(apply);
    } else if (state.mode === 'snapshot') {
      const execBtn = element('button', '执行');
      execBtn.dataset.operation = 'live-exec';
      execBtn.setAttribute('aria-label', `执行：${label}`);
      setActionGateHint(execBtn, state.board);
      execBtn.addEventListener('click', () => sendLiveCommand(action.kind, action.sourceEntityId ?? null, action.targetEntityId ?? null, action.description || label));
      row.append(execBtn);
    }
    list.append(row);
  });
  list.scrollTop = scroll;
}

function renderEvents(state, names) {
  const list = byId('events');
  list.replaceChildren();
  if (!state.events?.length) list.append(element('li', '推演动作后，按结算顺序显示事件。', 'empty'));
  for (const entry of state.events ?? []) {
    const event = entry.event;
    const entity = id => names.has(id.value) ? `${names.get(id.value)} #${id.value}` : `实体 #${id.value}`;
    const label = [eventNames[event.kind] || '其他结算', event.player ? `玩家 ${event.player.value}` : '',
      event.source ? `来源 ${entity(event.source)}` : '', event.target ? `目标 ${entity(event.target)}` : '',
      event.amount == null ? '' : `数值 ${event.amount}`, event.enchantmentCardId ?? ''].filter(Boolean).join(' / ');
    const row = element('li');
    row.append(element('span', `序号 ${entry.sequence}`, 'event-number'), element('span', label));
    list.append(row);
  }
  list.scrollTop = list.scrollHeight;
}

function showState(state) {
  const signature = JSON.stringify([state.version, state.automatic, state.busy, state.decision, state.decisionError,
    state.autoPilot, state.lastCommandAck, state.lastCommandAction]);
  if (signature === stateKey) return;
  const boardChanged = state.version !== current?.version;
  const matchChanged = state.matchId !== current?.matchId || state.mode !== current?.mode;
  stateKey = signature;
  current = state;
  const names = entityNames(state.board);
  if (boardChanged) { updateEntities(state.board, matchChanged); renderBoard(state); renderEvents(state, names); }
  renderDecision(state, names);
  paintControls();
}

function showConnection(value) {
  const signature = JSON.stringify(value);
  if (signature === bindingKey) return;
  bindingKey = signature;
  binding = value;
  const status = byId('connection-status');
  status.textContent = binding?.injected ? '实时监听中' : (({ online: '已绑定', offline: '已离线', unknown: '无法确认' })[binding?.status] || '未绑定');
  status.dataset.live = binding?.status === 'online';
  const node = byId('binding');
  const wasOpen = node.querySelector('details')?.open;
  node.replaceChildren();
  if (binding) {
    const { client, mainModuleSignature, modules } = binding.inspection;
    node.append(element('p', `PID ${client.pid} · 启动标识 ${client.startTime} · ${client.architecture} · 签名 ${mainModuleSignature}${binding.injected ? ' · 已注入 Silverfish.GameProbe' : ''}`, 'muted'));
    node.append(element('p', client.executablePath, 'muted'));
    const details = element('details');
    details.open = wasOpen ?? false;
    const list = element('ul');
    for (const name of modules) list.append(element('li', name));
    details.append(element('summary', `已检查的模块（${modules.length}）`), list);
    node.append(details);
  }
  paintControls();
}

async function listClients() {
  const node = byId('client-list');
  const result = await api('clients');
  node.replaceChildren();
  if (!result.clients.length) node.append(element('p', result.unavailable || '未发现正在运行的炉石客户端。', 'empty'));
  for (const client of result.clients) {
    const row = element('div', null, 'client-row');
    const name = element('span', `PID ${client.pid} · ${client.architecture}（${client.bits} 位）`);
    name.append(element('small', `启动标识：${client.startTime}`), element('small', client.executablePath));
    const button = element('button', '绑定并检查');
    button.dataset.operation = 'bind';
    button.addEventListener('click', () => operate(async () => {
      showConnection(await api('bind', JSON.stringify({ pid: client.pid, startTime: client.startTime })));
      notice('已绑定实例！现在可以点击「2. 注入实时监听」实时接收游戏内牌面与 EntityMap。');
    }));
    row.append(name, button);
    node.append(row);
  }
}

async function poll() {
  if (mutation) return;
  const startedAt = pollVersion;
  try {
    const response = await request('state', { cache: 'no-store', headers: stateEtag ? { 'If-None-Match': stateEtag } : {} });
    if (response.status === 304) return;
    const state = await response.json();
    if (!mutation && startedAt === pollVersion) { showState(state); stateEtag = response.headers.get('ETag') || ''; }
  } catch (error) {
    if (startedAt === pollVersion && !mutation) notice(error.message, true);
  }
}

async function pollConnection() {
  if (mutation) return;
  const startedAt = pollVersion;
  try {
    const connection = await api('connection');
    if (!mutation && startedAt === pollVersion) showConnection(connection);
  } catch (error) {
    if (!mutation && startedAt === pollVersion) notice(error.message, true);
  }
}

for (const [id, event] of [['entity-search', 'input'], ['entity-zone', 'change']]) {
  byId(id).addEventListener(event, () => { entityLimit = 100; renderEntityList(); });
}
byId('entity-more').addEventListener('click', () => { entityLimit += 100; renderEntityList(); });
byId('samples').addEventListener('change', paintControls);
byId('load-sample').addEventListener('click', () => change('sample', { name: byId('samples').value }, '已载入样例。'));
byId('reset').addEventListener('click', () => change('reset', {}, '已清空局面并停止自动分析和托管。'));
byId('decide').addEventListener('click', () => change('decide', { version: current.version }));
byId('automatic').addEventListener('click', () => change('automatic', { enabled: !current?.automatic }));
byId('autopilot').addEventListener('click', () => change('autopilot', { enabled: !current?.autoPilot }));
byId('refresh-clients').addEventListener('click', () => operate(listClients));
byId('confirm-choice').addEventListener('click', () => sendLiveCommand('confirm_choice', null, null, '确认选择'));
byId('live-end-turn').addEventListener('click', () => sendLiveCommand('end_turn', null, null, '结束回合'));
byId('inject-client').addEventListener('click', () => operate(async () => {
  showConnection(await api('inject', '{}'));
  notice('已向炉石进程注入 Silverfish.GameProbe 实时监听器！进入对局后牌面会自动刷新。');
}));
byId('disconnect').addEventListener('click', () => operate(async () => showConnection(await api('disconnect', '{}'))));
byId('open-import').addEventListener('click', () => { notice('', false, 'import-error'); byId('import-dialog').showModal(); });
byId('close-import').addEventListener('click', () => byId('import-dialog').close());
byId('import-file').addEventListener('change', async () => {
  const file = byId('import-file').files[0];
  if (!file) return;
  try {
    if (file.size > 2 * 1024 * 1024) throw new Error('输入不能超过 2 MiB。');
    byId('import-json').value = await file.text();
    notice('', false, 'import-error');
  } catch (error) { notice(error.message, true, 'import-error'); }
});
byId('import-submit').addEventListener('click', () => operate(async () => {
  const text = byId('import-json').value.trim();
  if (!text) throw new Error('请选取文件或粘贴 JSON。');
  const matchId = byId('match-id').value.trim();
  showState(await api(`import?matchId=${encodeURIComponent(matchId)}`, text));
  byId('import-dialog').close();
  byId('import-file').value = '';
  byId('import-json').value = '';
  notice('已导入局面。');
}, 'import-error'));

api('samples').then(samples => {
  const select = byId('samples');
  select.replaceChildren();
  const groups = { simulation: '离线模拟', snapshot: '可观察快照', client: '客户端快照' };
  for (const [kind, label] of Object.entries(groups)) {
    const group = element('optgroup');
    group.label = label;
    for (const sample of samples.filter(sample => sample.kind === kind)) {
      const option = element('option', sampleNames[sample.name] || sample.name);
      option.value = sample.name;
      option.title = sample.name;
      group.append(option);
    }
    select.append(group);
  }
  select.value = 'simulation-combat.json';
  if (!select.value) select.selectedIndex = 0;
  paintControls();
}).catch(error => notice(error.message, true));

(async function refresh() { await poll(); setTimeout(refresh, document.hidden ? 2500 : 400); })();
(async function refreshConnection() { await pollConnection(); setTimeout(refreshConnection, 4000); })();
