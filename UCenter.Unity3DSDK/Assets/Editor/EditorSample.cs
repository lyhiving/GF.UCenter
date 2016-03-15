using UnityEditor;
using UnityEngine;
using System.Collections;

public class EditorSample : EditorWindow
{
    //-------------------------------------------------------------------------
    [MenuItem("UCenter/导出GfUCenter.unitypackage")]
    static void exportGfJsonPackage()
    {
        string[] arr_assetpathname = new string[1];
        arr_assetpathname[0] = "Assets/GfUCenter";
        AssetDatabase.ExportPackage(arr_assetpathname, "GfUCenter.unitypackage", ExportPackageOptions.Recurse);

        Debug.Log("Export 导出GfUCenter.unitypackage Finished!");
    }
}
