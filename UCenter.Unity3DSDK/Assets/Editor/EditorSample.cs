using UnityEditor;
using UnityEngine;
using System.Collections;

public class EditorSample : EditorWindow
{
    //-------------------------------------------------------------------------
    [MenuItem("UCenter/导出UCenterUnity3DSDK.unitypackage")]
    static void exportGfJsonPackage()
    {
        string[] arr_assetpathname = new string[1];
        arr_assetpathname[0] = "Assets/UCenterUnity3DSDK";
        AssetDatabase.ExportPackage(arr_assetpathname, "UCenterUnity3DSDK.unitypackage", ExportPackageOptions.Recurse);

        Debug.Log("Export UCenterUnity3DSDK.unitypackage Finished!");
    }
}
