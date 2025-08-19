# ReadTableSystem 鎵╁睍

Provides data table reading functionality, supporting attribute and effect data caching and querying

## 鍔熻兘鐗规€?
- 馃敡 **鏍稿績鍔熻兘**锛?(System.Collections.Hashtable.Description)
- 馃攲 **鎵╁睍鐐?*锛氭敮鎸佸叾浠栨墿灞曠洃鍚郴缁熶簨浠?- 馃摝 **鑷姩娉ㄥ唽**锛氳嚜鍔ㄦ敞鍐屾湇鍔″拰鍛戒护澶勭悊鍣?- 馃幆 **浼樺厛绾?*锛?(System.Collections.Hashtable.Priority)

## 浣跨敤鏂规硶

### 1. 娉ㄥ唽鎵╁睍

`csharp
public partial class Main : Node
{
    public override void _Ready()
    {
        // 娉ㄥ唽ReadTableSystem鎵╁睍
        ExtendedContexts.Instance.RegisterExtension<ReadTableSystemExtension>();
        
        // 鑾峰彇鏈嶅姟锛堝鏋滄湁鐨勮瘽锛?        // var service = ExtendedContexts.Instance.GetService<IYourService>();
    }
}
`

### 2. 鎵╁睍鐐圭洃鍚?
`csharp
public class MyReadTableSystemExtension : FrameworkExtensionBase, IReadTableSystemExtensionPoint
{
    public override void Initialize(IExtensionContext context)
    {
        // 娉ㄥ唽涓烘墿灞曠偣
        RegisterExtensionPoint<IReadTableSystemExtensionPoint>(this);
    }
    
    public void OnSystemInitialized()
    {
        GD.Print("ReadTableSystem system initialized");
    }
    
    public void OnSystemStarted()
    {
        GD.Print("ReadTableSystem system started");
    }
    
    public void OnSystemStopped()
    {
        GD.Print("ReadTableSystem system stopped");
    }
}
`

## 鍖呭惈鐨勭郴缁?
- AttributeDatabaseReadService
- GameplayEffectDatabaseReadService


## 娉ㄦ剰浜嬮」

1. 纭繚鍦ㄤ娇鐢ㄥ墠娉ㄥ唽鎵╁睍
2. 鎵╁睍鎸変紭鍏堢骇鍔犺浇
3. 娉ㄦ剰鎵╁睍闂寸殑渚濊禆鍏崇郴
4. 姝ｇ‘澶勭悊璧勬簮娓呯悊
