using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreditRollEditor : Editor
{
    const string extension = ".cs";
    public static void WriteToEnum<T>(string path, string name, ICollection<T> data)
    {
        using (StreamWriter file = File.CreateText(path + name + extension))
        {
            file.WriteLine("public enum " + name + " \n{");

            int i = 0;
            foreach (var line in data)
            {
                string lineRep = line.ToString().Replace(" ", string.Empty);
                if (!string.IsNullOrEmpty(lineRep))
                {
                    file.WriteLine(string.Format("\t{0} = {1},",
                        lineRep, i));
                    i++;
                }
            }

            file.WriteLine("\n}");
        }

        AssetDatabase.ImportAsset(path + name + extension);
    }
}

[CustomEditor(typeof(CreditRoll))]
public class TestWriter : Editor
{
    CreditRoll myScrip;
    string filePath = "Assets/";
    string fileName = "FontType";

    private void OnEnable()
    {
        myScrip = (CreditRoll)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        filePath = EditorGUILayout.TextField("Generate Location: ", filePath);
        fileName = EditorGUILayout.TextField("Generate Font File Name: ", fileName);
        if (GUILayout.Button("Apply Font Changes"))
        {
            CreditRollEditor.WriteToEnum(filePath, fileName, myScrip.fontName);
        }
    }
}
