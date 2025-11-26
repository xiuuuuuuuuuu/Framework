using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
public class GeneratorFindComponentTool : Editor
{
    public static Dictionary<int, string> objFindPaathDic;// key:InstanceID value:对象在层级中的路径
    public static List<EditorObjectData> objDataList;//存储对象数据的列表

    [MenuItem("ZMUI/生成查找组件脚本")]
    static void CreateFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject;// 获取当前选择的物体
        if (obj == null)
        {
            Debug.LogError("请选择 GameObject");
            return;
        }
        objDataList = new();
        objFindPaathDic = new();

        //设置脚本生成路径
        //检查指定目录（文件夹）是否存在 如果不存在则创建
        if (!Directory.Exists(GeneratorConfig.FindComponentGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.FindComponentGeneratorPath);
        }
    }

    
}
public class EditorObjectData
{
    public int instanceID;
    public string objName;
    public string objPath;
}
