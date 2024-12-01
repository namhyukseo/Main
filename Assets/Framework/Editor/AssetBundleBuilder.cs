using UnityEngine;
using UnityEditor;

public class AssetBundleBuilder
{
    public static bool useSimulationMode = false;
    [MenuItem("Build/Bundle Mode")]
    public static void ToggleBundleMode()
    {
        useSimulationMode = !useSimulationMode;
    }

    [MenuItem("Build/Bundle Mode",true)]
    public static bool ToggleBundleModeValidate()
    {
        Menu.SetChecked("Build/Bundle Mode", useSimulationMode);
        return true;
    }

    [MenuItem("Build/Bundle Build")]
    public static void BundleBuild()
    {
        
    }
}
