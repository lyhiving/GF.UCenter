using UnityEditor;
using UnityEngine;
using System.Collections;

public class EditorSample : EditorWindow
{
    //-------------------------------------------------------------------------
    [MenuItem("UCenter/导出GF.UCenter.unitypackage")]
    static void exportGFUCenterPackage()
    {
        string[] arr_assetpathname = new string[1];
        arr_assetpathname[0] = "Assets/GF.UCenter";
        AssetDatabase.ExportPackage(arr_assetpathname, "GF.UCenter.unitypackage", ExportPackageOptions.Recurse);

        Debug.Log("Export 导出GF.UCenter.unitypackage Finished!");
    }
}
