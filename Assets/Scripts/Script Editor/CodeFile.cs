using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CodeFile
{
    public string Name;
    public string Script;
    [System.NonSerialized]
    public List<ScriptLayer> Layers;
    [System.NonSerialized]
    public bool IsNonSelectableItemUI;

    public CodeFile(string name, string script)
    {
        Name = name;
        Script = script;
        Layers = new List<ScriptLayer>();
        SetupLayer();
    }

    public string Save()
    {
        return JsonUtility.ToJson(this);
    }
    public void SetupLayer()
    {
        char[] chars = { '\n' };
        Layers.Clear();
        ScriptLayer layer = new ScriptLayer()
        {
            TypeKey = Name,
            Lines = Script.Split(chars, System.StringSplitOptions.RemoveEmptyEntries)
        };
        Layers.Add(layer);
    }
    public List<ScriptLayer> CopyLayers()
    {
        List<ScriptLayer> _layers = new List<ScriptLayer>(Layers.Count);
        for (int i = 0; i < Layers.Count; i++)
        {
            ScriptLayer _l = Layers[i];
            ScriptLayer new_l = new ScriptLayer()
            {
                TypeKey = _l.TypeKey,
                Lines = new string[_l.Lines.Length]
            };
            for (int j = 0; j < _l.Lines.Length; j++)
            {
                new_l.Lines[j] = _l.Lines[j];
            }
            _layers.Add(new_l);
        }
        return _layers;
    }
    public void ExtendLayer(List<ScriptLayer> _layers) => Layers = _layers.Concat(Layers).ToList();
    public static CodeFile Load(string json)
    {
        CodeFile file = JsonUtility.FromJson<CodeFile>(json);
        file.Layers = new List<ScriptLayer>();
        file.SetupLayer();
        return file;
    }
}
public class ScriptLayer
{
    public string TypeKey;
    public string[] Lines;
}
