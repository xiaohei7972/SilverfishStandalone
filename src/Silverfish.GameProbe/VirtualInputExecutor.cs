using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace Silverfish.GameProbe;

public sealed class ProbeCommand
{
    public string Id { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public int? SourceEntityId { get; set; }
    public int? TargetEntityId { get; set; }
    public int? Position { get; set; }
}

public static class VirtualInputState
{
    public static volatile bool Active;
    public static float MouseX;
    public static float MouseY;
    public static volatile bool LeftHeld;
    public static volatile bool LeftDown;
    public static volatile bool LeftUp;

    public static void Reset()
    {
        Active = false;
        LeftHeld = false;
        LeftDown = false;
        LeftUp = false;
    }

    public static bool HandleGetMousePosition(out float x, out float y)
    {
        VirtualInputExecutor.TickFromUnityInputPoll();
        if (!Active)
        {
            x = 0;
            y = 0;
            return false;
        }
        x = MouseX;
        y = MouseY;
        return true;
    }

    public static bool HandleGetAnyKey(out bool value)
    {
        value = false;
        return Active;
    }

    public static bool HandleGetKey(int keycode, out bool value)
    {
        value = false;
        return Active;
    }

    public static bool HandleGetMouseButton(int button, out bool value)
    {
        if (!Active)
        {
            value = false;
            return false;
        }
        value = button == 0 && LeftHeld;
        return true;
    }

    public static bool HandleGetMouseButtonDown(int button, out bool value)
    {
        if (!Active)
        {
            value = false;
            return false;
        }
        value = button == 0 && LeftDown;
        return true;
    }

    public static bool HandleGetMouseButtonUp(int button, out bool value)
    {
        if (!Active)
        {
            value = false;
            return false;
        }
        value = button == 0 && LeftUp;
        return true;
    }
}

internal static class VirtualInputExecutor
{
    private const BindingFlags AnyStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private static readonly ConcurrentQueue<ProbeCommand> Queue = new ConcurrentQueue<ProbeCommand>();
    private static readonly object InstallSync = new object();
    private static Action<string, bool, string>? _ackCallback;
    private static Action<string>? _logCallback;
    private static bool _installed;
    private static int _lastUnityFrame = -1;
    private static PropertyInfo? _frameCountProp;

    private static ProbeCommand? _activeCommand;
    private static bool _isDrag;
    private static float _fromX, _fromY, _toX, _toY;
    private static int _frameStep;

    internal static bool IsExecuting => _activeCommand != null || VirtualInputState.Active;

    internal static void Initialize(Action<string, bool, string> ackCallback, Action<string> logCallback)
    {
        _ackCallback = ackCallback;
        _logCallback = logCallback;
        EnsureRunnerInstalled();
    }

    internal static void Enqueue(ProbeCommand command)
    {
        Queue.Enqueue(command);
        EnsureRunnerInstalled();
    }

    internal static void EnsureRunnerInstalled()
    {
        lock (InstallSync)
        {
            if (!_installed)
            {
                try
                {
                    InstallVirtualInputIntoInputCollection();
                    _installed = true;
                }
                catch (Exception ex)
                {
                    _logCallback?.Invoke("InstallVirtualInput failed: " + ex.Message);
                }
            }
        }
    }

    internal static void TickFromUnityInputPoll()
    {
        try
        {
            if (_frameCountProp == null)
            {
                var timeType = GameStateProbeCollector.FindType("UnityEngine.Time");
                _frameCountProp = timeType?.GetProperty("frameCount", AnyStatic);
            }
            var currentFrame = _frameCountProp != null ? Convert.ToInt32(_frameCountProp.GetValue(null, null), CultureInfo.InvariantCulture) : -1;
            if (currentFrame >= 0 && currentFrame == _lastUnityFrame)
                return;
            _lastUnityFrame = currentFrame;
            OnUnityUpdate();
        }
        catch { }
    }

    public static void OnUnityUpdate()
    {
        try
        {
            // Clear single-frame edge pulses at the start of each frame before advancing state
            VirtualInputState.LeftDown = false;
            VirtualInputState.LeftUp = false;

            if (_activeCommand == null)
            {
                if (!Queue.TryDequeue(out var next))
                    return;

                StartCommandOnMainThread(next);
                return;
            }

            AdvanceCommandFrame();
        }
        catch (Exception ex)
        {
            var cmdId = _activeCommand?.Id ?? string.Empty;
            FinishCommand(cmdId, false, "执行异常：" + ex.Message);
        }
    }

    private static void StartCommandOnMainThread(ProbeCommand cmd)
    {
        _logCallback?.Invoke($"Executing command {cmd.Id}: {cmd.Kind} src={cmd.SourceEntityId} tgt={cmd.TargetEntityId}");
        try
        {
            switch (cmd.Kind)
            {
                case "choose_entity":
                {
                    var id = cmd.SourceEntityId ?? cmd.TargetEntityId ?? 0;
                    if (id <= 0 || !TryGetChoiceCardScreenPoint(id, out _fromX, out _fromY))
                    {
                        _ackCallback?.Invoke(cmd.Id, false, $"找不到发现/选择卡牌 #{id} 的屏幕位置。");
                        return;
                    }
                    BeginClick(cmd, _fromX, _fromY);
                    break;
                }
                case "confirm_choice":
                {
                    if (!TryGetConfirmButtonScreenPoint(out _fromX, out _fromY))
                    {
                        _ackCallback?.Invoke(cmd.Id, false, "当前界面没有可见的确认按钮。");
                        return;
                    }
                    BeginClick(cmd, _fromX, _fromY);
                    break;
                }
                case "end_turn":
                case "endTurn":
                {
                    if (!TryGetEndTurnButtonScreenPoint(out _fromX, out _fromY))
                    {
                        _ackCallback?.Invoke(cmd.Id, false, "找不到结束回合按钮或当前不可点击。");
                        return;
                    }
                    BeginClick(cmd, _fromX, _fromY);
                    break;
                }
                case "play_card":
                case "playCard":
                {
                    var srcId = cmd.SourceEntityId ?? 0;
                    if (srcId <= 0 || !TryGetEntityScreenPoint(srcId, isHandCard: true, out _fromX, out _fromY))
                    {
                        _ackCallback?.Invoke(cmd.Id, false, $"找不到手牌 #{srcId} 的屏幕位置。");
                        return;
                    }
                    if (cmd.TargetEntityId is int tgtId && tgtId > 0)
                    {
                        if (!TryGetEntityScreenPoint(tgtId, isHandCard: false, out _toX, out _toY))
                        {
                            _ackCallback?.Invoke(cmd.Id, false, $"找不到目标 #{tgtId} 的屏幕位置。");
                            return;
                        }
                    }
                    else
                    {
                        if (!TryGetFriendlyBoardCenterScreenPoint(out _toX, out _toY))
                        {
                            _ackCallback?.Invoke(cmd.Id, false, "找不到我方战场落点位置。");
                            return;
                        }
                    }
                    BeginDrag(cmd, _fromX, _fromY, _toX, _toY);
                    break;
                }
                case "attack":
                {
                    var srcId = cmd.SourceEntityId ?? 0;
                    var tgtId = cmd.TargetEntityId ?? 0;
                    if (srcId <= 0 || tgtId <= 0 ||
                        !TryGetEntityScreenPoint(srcId, isHandCard: false, out _fromX, out _fromY) ||
                        !TryGetEntityScreenPoint(tgtId, isHandCard: false, out _toX, out _toY))
                    {
                        _ackCallback?.Invoke(cmd.Id, false, $"找不到攻击来源 #{srcId} 或目标 #{tgtId} 的屏幕位置。");
                        return;
                    }
                    BeginDrag(cmd, _fromX, _fromY, _toX, _toY);
                    break;
                }
                case "hero_power":
                case "heroPower":
                case "useHeroPower":
                case "activate_location":
                case "activateLocation":
                {
                    var srcId = cmd.SourceEntityId ?? 0;
                    if (srcId <= 0 || !TryGetEntityScreenPoint(srcId, isHandCard: false, out _fromX, out _fromY))
                    {
                        _ackCallback?.Invoke(cmd.Id, false, $"找不到英雄技能/地标 #{srcId} 的屏幕位置。");
                        return;
                    }
                    if (cmd.TargetEntityId is int tgtId && tgtId > 0)
                    {
                        if (!TryGetEntityScreenPoint(tgtId, isHandCard: false, out _toX, out _toY))
                        {
                            _ackCallback?.Invoke(cmd.Id, false, $"找不到目标 #{tgtId} 的屏幕位置。");
                            return;
                        }
                        BeginDrag(cmd, _fromX, _fromY, _toX, _toY);
                    }
                    else
                    {
                        BeginClick(cmd, _fromX, _fromY);
                    }
                    break;
                }
                default:
                    _ackCallback?.Invoke(cmd.Id, false, $"不支持的操作类型：{cmd.Kind}");
                    break;
            }
        }
        catch (Exception ex)
        {
            _ackCallback?.Invoke(cmd.Id, false, "定位目标失败：" + ex.Message);
        }
    }

    private static void BeginClick(ProbeCommand cmd, float x, float y)
    {
        EnsurePegUiFocus();
        _activeCommand = cmd;
        _isDrag = false;
        _fromX = _toX = x;
        _fromY = _toY = y;
        _frameStep = 0;
        VirtualInputState.MouseX = x;
        VirtualInputState.MouseY = y;
        VirtualInputState.LeftHeld = false;
        VirtualInputState.LeftDown = false;
        VirtualInputState.LeftUp = false;
        VirtualInputState.Active = true;
        _logCallback?.Invoke($"BeginClick id={cmd.Id} screen=({x:F1}, {y:F1})");
    }

    private static void BeginDrag(ProbeCommand cmd, float fromX, float fromY, float toX, float toY)
    {
        EnsurePegUiFocus();
        _activeCommand = cmd;
        _isDrag = true;
        _fromX = fromX;
        _fromY = fromY;
        _toX = toX;
        _toY = toY;
        _frameStep = 0;
        VirtualInputState.MouseX = fromX;
        VirtualInputState.MouseY = fromY;
        VirtualInputState.LeftHeld = false;
        VirtualInputState.LeftDown = false;
        VirtualInputState.LeftUp = false;
        VirtualInputState.Active = true;
        _logCallback?.Invoke($"BeginDrag id={cmd.Id} from=({fromX:F1},{fromY:F1}) to=({toX:F1},{toY:F1})");
    }

    private static void EnsurePegUiFocus()
    {
        try
        {
            var pegUiType = GameStateProbeCollector.FindType("PegUI");
            var pegUi = pegUiType?.GetMethod("Get", AnyStatic)?.Invoke(null, null);
            if (pegUi != null)
                pegUiType!.GetField("m_hasFocus", AnyInstance)?.SetValue(pegUi, true);
        }
        catch { }
    }

    private static void AdvanceCommandFrame()
    {
        _frameStep++;
        if (!_isDrag)
        {
            if (_frameStep == 2)
            {
                VirtualInputState.LeftDown = true;
                VirtualInputState.LeftHeld = true;
            }
            else if (_frameStep == 5)
            {
                VirtualInputState.LeftHeld = false;
                VirtualInputState.LeftUp = true;
            }
            else if (_frameStep >= 8)
            {
                FinishCommand(_activeCommand!.Id, true, $"已完成点击 ({_fromX:F0}, {_fromY:F0})。");
            }
        }
        else
        {
            if (_frameStep == 2)
            {
                VirtualInputState.LeftDown = true;
                VirtualInputState.LeftHeld = true;
            }
            else if (_frameStep >= 4 && _frameStep <= 12)
            {
                var t = (_frameStep - 4) / 8f;
                VirtualInputState.MouseX = _fromX + ((_toX - _fromX) * t);
                VirtualInputState.MouseY = _fromY + ((_toY - _fromY) * t);
            }
            else if (_frameStep == 14)
            {
                VirtualInputState.MouseX = _toX;
                VirtualInputState.MouseY = _toY;
                VirtualInputState.LeftHeld = false;
                VirtualInputState.LeftUp = true;
            }
            else if (_frameStep >= 17)
            {
                FinishCommand(_activeCommand!.Id, true, $"已完成拖拽 ({_fromX:F0},{_fromY:F0}) -> ({_toX:F0},{_toY:F0})。");
            }
        }
    }

    private static void FinishCommand(string cmdId, bool ok, string message)
    {
        VirtualInputState.Reset();
        _activeCommand = null;
        _frameStep = 0;
        ProbeEntry.MarkDirty();
        if (!string.IsNullOrEmpty(cmdId))
            _ackCallback?.Invoke(cmdId, ok, message);
    }

    private static bool TryGetChoiceCardScreenPoint(int entityId, out float x, out float y)
    {
        x = y = 0;
        var mgrType = GameStateProbeCollector.FindType("ChoiceCardMgr");
        var mgr = mgrType?.GetMethod("Get", AnyStatic)?.Invoke(null, null);
        if (mgr == null) return false;

        if (GameStateProbeCollector.Invoke(mgr, "GetFriendlyCards") is IEnumerable cards)
        {
            foreach (var card in cards)
            {
                if (card == null) continue;
                var ent = GameStateProbeCollector.Invoke(card, "GetEntity");
                var id = ent != null ? Convert.ToInt32(GameStateProbeCollector.Invoke(ent, "GetEntityId") ?? 0, CultureInfo.InvariantCulture) : 0;
                if (id == entityId && TryGetComponentScreenPoint(card, out x, out y))
                    return true;
            }
        }
        return false;
    }

    private static bool TryGetConfirmButtonScreenPoint(out float x, out float y)
    {
        x = y = 0;
        var mgrType = GameStateProbeCollector.FindType("ChoiceCardMgr");
        var mgr = mgrType?.GetMethod("Get", AnyStatic)?.Invoke(null, null);
        if (mgr == null) return false;
        var btn = GameStateProbeCollector.GetFieldOrProperty(mgr, "m_confirmChoiceButton");
        return btn != null && TryGetComponentScreenPoint(btn, out x, out y);
    }

    private static bool TryGetEndTurnButtonScreenPoint(out float x, out float y)
    {
        x = y = 0;
        var btnType = GameStateProbeCollector.FindType("EndTurnButton");
        var btn = btnType?.GetMethod("Get", AnyStatic)?.Invoke(null, null);
        return btn != null && TryGetComponentScreenPoint(btn, out x, out y);
    }

    private static bool TryGetFriendlyBoardCenterScreenPoint(out float x, out float y)
    {
        x = y = 0;
        var gsType = GameStateProbeCollector.FindGameStateType();
        var gs = gsType?.GetMethod("Get", AnyStatic)?.Invoke(null, null);
        var friendly = gs != null ? GameStateProbeCollector.Invoke(gs, "GetFriendlySidePlayer") : null;
        var zone = friendly != null ? GameStateProbeCollector.Invoke(friendly, "GetBattlefieldZone") : null;
        return zone != null && TryGetComponentScreenPoint(zone, out x, out y);
    }

    private static bool TryGetEntityScreenPoint(int entityId, bool isHandCard, out float x, out float y)
    {
        x = y = 0;
        var gsType = GameStateProbeCollector.FindGameStateType();
        var gs = gsType?.GetMethod("Get", AnyStatic)?.Invoke(null, null);
        if (gs == null) return false;

        var getEntity = gs.GetType().GetMethod("GetEntity", AnyInstance, null, new[] { typeof(int) }, null);
        var entity = getEntity?.Invoke(gs, new object[] { entityId });
        if (entity == null) return false;

        var card = GameStateProbeCollector.Invoke(entity, "GetCard");
        if (card == null) return false;

        if (isHandCard)
        {
            var zone = GameStateProbeCollector.Invoke(card, "GetZone");
            var getStandIn = zone?.GetType().GetMethod("GetStandIn", AnyInstance);
            if (getStandIn != null && getStandIn.GetParameters().Length == 1)
            {
                var standIn = getStandIn.Invoke(zone, new object[] { card });
                if (standIn != null && TryGetComponentScreenPoint(standIn, out x, out y))
                    return true;
            }
        }

        return TryGetComponentScreenPoint(card, out x, out y);
    }

    private static bool TryGetComponentScreenPoint(object unityComponent, out float x, out float y)
    {
        x = y = 0;
        if (unityComponent == null || string.Equals(unityComponent.ToString(), "null", StringComparison.OrdinalIgnoreCase))
            return false;

        // Prefer BoxCollider / Collider bounds.center when available, else transform.position
        object? worldPos = null;
        try
        {
            var getComp = unityComponent.GetType().GetMethod("GetComponentInChildren", AnyInstance, null, new[] { typeof(Type) }, null);
            var colliderType = GameStateProbeCollector.FindType("UnityEngine.Collider");
            if (getComp != null && colliderType != null)
            {
                var col = getComp.Invoke(unityComponent, new object[] { colliderType });
                if (col != null && !string.Equals(col.ToString(), "null", StringComparison.OrdinalIgnoreCase))
                {
                    var bounds = GameStateProbeCollector.GetFieldOrProperty(col, "bounds");
                    if (bounds != null)
                        worldPos = GameStateProbeCollector.GetFieldOrProperty(bounds, "center");
                }
            }
        }
        catch { }

        if (worldPos == null)
        {
            var transform = GameStateProbeCollector.GetFieldOrProperty(unityComponent, "transform");
            if (transform == null) return false;
            worldPos = GameStateProbeCollector.GetFieldOrProperty(transform, "position");
            if (worldPos == null) return false;
        }

        var cameraType = GameStateProbeCollector.FindType("UnityEngine.Camera");
        var mainCam = cameraType?.GetProperty("main", AnyStatic)?.GetValue(null, null);
        if (mainCam == null) return false;

        var worldToScreen = cameraType!.GetMethod("WorldToScreenPoint", AnyInstance, null, new[] { worldPos.GetType() }, null);
        var screenPos = worldToScreen?.Invoke(mainCam, new[] { worldPos });
        if (screenPos == null) return false;

        x = Convert.ToSingle(GameStateProbeCollector.GetFieldOrProperty(screenPos, "x"), CultureInfo.InvariantCulture);
        y = Convert.ToSingle(GameStateProbeCollector.GetFieldOrProperty(screenPos, "y"), CultureInfo.InvariantCulture);
        return true;
    }

    private static void InstallVirtualInputIntoInputCollection()
    {
        var inputCollectionType = GameStateProbeCollector.FindType("InputCollection")
            ?? throw new InvalidOperationException("找不到 InputCollection 类型。");
        var iInputType = GameStateProbeCollector.FindType("IInput")
            ?? throw new InvalidOperationException("找不到 IInput 接口。");
        var vector3Type = GameStateProbeCollector.FindType("UnityEngine.Vector3")
            ?? throw new InvalidOperationException("找不到 UnityEngine.Vector3 类型。");
        var keyCodeType = GameStateProbeCollector.FindType("UnityEngine.KeyCode")
            ?? throw new InvalidOperationException("找不到 UnityEngine.KeyCode 类型。");

        var asmName = new AssemblyName("SilverfishVirtualInputProxy_" + Guid.NewGuid().ToString("N"));
        var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
        var modBuilder = asmBuilder.DefineDynamicModule("Main");
        var tb = modBuilder.DefineType("SilverfishVirtualInput", TypeAttributes.Public | TypeAttributes.Class, typeof(object), new[] { iInputType });

        // bool GetMousePosition(out Vector3 position)
        var getMousePos = tb.DefineMethod("GetMousePosition", MethodAttributes.Public | MethodAttributes.Virtual, typeof(bool), new[] { vector3Type.MakeByRefType() });
        {
            var il = getMousePos.GetILGenerator();
            var xLocal = il.DeclareLocal(typeof(float));
            var yLocal = il.DeclareLocal(typeof(float));
            var activeLocal = il.DeclareLocal(typeof(bool));
            var vecCtor = vector3Type.GetConstructor(new[] { typeof(float), typeof(float), typeof(float) })!;
            il.Emit(OpCodes.Ldloca, xLocal);
            il.Emit(OpCodes.Ldloca, yLocal);
            il.Emit(OpCodes.Call, typeof(VirtualInputState).GetMethod(nameof(VirtualInputState.HandleGetMousePosition))!);
            il.Emit(OpCodes.Stloc, activeLocal);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, xLocal);
            il.Emit(OpCodes.Ldloc, yLocal);
            il.Emit(OpCodes.Ldc_R4, 0f);
            il.Emit(OpCodes.Newobj, vecCtor);
            il.Emit(OpCodes.Stobj, vector3Type);
            il.Emit(OpCodes.Ldloc, activeLocal);
            il.Emit(OpCodes.Ret);
        }

        // bool GetAnyKey(out bool value)
        DefineForwardMethod(tb, "GetAnyKey", new[] { typeof(bool).MakeByRefType() },
            typeof(VirtualInputState).GetMethod(nameof(VirtualInputState.HandleGetAnyKey))!);

        // bool GetKey / GetKeyDown / GetKeyUp (KeyCode, out bool)
        var keyHandler = typeof(VirtualInputState).GetMethod(nameof(VirtualInputState.HandleGetKey))!;
        DefineForwardMethod(tb, "GetKey", new[] { keyCodeType, typeof(bool).MakeByRefType() }, keyHandler);
        DefineForwardMethod(tb, "GetKeyDown", new[] { keyCodeType, typeof(bool).MakeByRefType() }, keyHandler);
        DefineForwardMethod(tb, "GetKeyUp", new[] { keyCodeType, typeof(bool).MakeByRefType() }, keyHandler);

        // bool GetMouseButton / Down / Up (int, out bool)
        DefineForwardMethod(tb, "GetMouseButton", new[] { typeof(int), typeof(bool).MakeByRefType() },
            typeof(VirtualInputState).GetMethod(nameof(VirtualInputState.HandleGetMouseButton))!);
        DefineForwardMethod(tb, "GetMouseButtonDown", new[] { typeof(int), typeof(bool).MakeByRefType() },
            typeof(VirtualInputState).GetMethod(nameof(VirtualInputState.HandleGetMouseButtonDown))!);
        DefineForwardMethod(tb, "GetMouseButtonUp", new[] { typeof(int), typeof(bool).MakeByRefType() },
            typeof(VirtualInputState).GetMethod(nameof(VirtualInputState.HandleGetMouseButtonUp))!);

        var proxyType = tb.CreateTypeInfo()!.AsType();
        var proxyInstance = Activator.CreateInstance(proxyType)!;

        var listField = inputCollectionType.GetField("m_Inputs", AnyStatic)
            ?? throw new InvalidOperationException("找不到 InputCollection.m_Inputs 字段。");
        var list = listField.GetValue(null) as IList
            ?? throw new InvalidOperationException("InputCollection.m_Inputs 为空。");
        for (var i = list.Count - 1; i >= 0; i--)
        {
            if (list[i]?.GetType().Name == "SilverfishVirtualInput")
                list.RemoveAt(i);
        }
        list.Insert(0, proxyInstance);
        _logCallback?.Invoke("Installed SilverfishVirtualInput at InputCollection.m_Inputs[0]");
    }

    private static void DefineForwardMethod(TypeBuilder tb, string name, Type[] paramTypes, MethodInfo target)
    {
        var method = tb.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(bool), paramTypes);
        var il = method.GetILGenerator();
        for (var i = 0; i < paramTypes.Length; i++)
            il.Emit(OpCodes.Ldarg, i + 1);
        il.Emit(OpCodes.Call, target);
        il.Emit(OpCodes.Ret);
    }

    private static void AttachUnityMainThreadRunner()
    {
        var monoBehaviourType = GameStateProbeCollector.FindType("UnityEngine.MonoBehaviour")
            ?? throw new InvalidOperationException("找不到 UnityEngine.MonoBehaviour。");
        var gameObjectType = GameStateProbeCollector.FindType("UnityEngine.GameObject")
            ?? throw new InvalidOperationException("找不到 UnityEngine.GameObject。");
        var unityObjectType = GameStateProbeCollector.FindType("UnityEngine.Object")
            ?? throw new InvalidOperationException("找不到 UnityEngine.Object。");

        var asmName = new AssemblyName("SilverfishUnityRunner_" + Guid.NewGuid().ToString("N"));
        var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
        var modBuilder = asmBuilder.DefineDynamicModule("Main");
        var tb = modBuilder.DefineType("SilverfishProbeRunnerBehaviour", TypeAttributes.Public | TypeAttributes.Class, monoBehaviourType);

        var updateMethod = tb.DefineMethod("Update", MethodAttributes.Private, typeof(void), Type.EmptyTypes);
        var il = updateMethod.GetILGenerator();
        il.Emit(OpCodes.Call, typeof(VirtualInputExecutor).GetMethod(nameof(OnUnityUpdate), AnyStatic)!);
        il.Emit(OpCodes.Ret);

        var runnerType = tb.CreateTypeInfo()!.AsType();
        var go = Activator.CreateInstance(gameObjectType, new object[] { "SilverfishProbeRunner" })!;
        var dontDestroy = unityObjectType.GetMethod("DontDestroyOnLoad", AnyStatic);
        dontDestroy?.Invoke(null, new[] { go });
        var addComp = gameObjectType.GetMethod("AddComponent", AnyInstance, null, new[] { typeof(Type) }, null);
        addComp?.Invoke(go, new object[] { runnerType });
        _logCallback?.Invoke("Attached SilverfishProbeRunnerBehaviour to Unity main thread");
    }
}
