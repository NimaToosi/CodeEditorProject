using UnityEngine;
using NTL.ScriptEditor;

public class CodeEditor : MonoBehaviour
{
    public ScriptEditor ScriptEditor;
    private CodeFile currentCode;
    private string codeClipBoard;

    public bool Visible
    {
        get
        {
            return gameObject.activeSelf;
        }
        private set
        {
            gameObject.SetActive(value);
        }
    }

    void Start()
    {
        ScriptEditor.Initialize();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        ScriptEditor.enabled = false;
#endif
        //Visible = false;
        OpenCode(new CodeFile("code_1", "extends script;\n\nfunc update()\n\nend"));
    }

    public void OpenCode(CodeFile code)
    {
        currentCode = code;
        Visible = true;
        ScriptEditor.Load(code.Script);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        ScriptEditor.enabled = true;
#endif
    }
    public void Copy()
    {
        Clipboard.CopyScript(ScriptEditor.Lines);
    }
    public void Paste()
    {
        for (int i = 0; i < Clipboard.ScriptLines.Count; i++)
        {
            ScriptEditor.InputString(Clipboard.ScriptLines[i]);
            ScriptEditor.Submit();
        }
    }
    public void Clear()
    {
        ScriptEditor.Clear();
    }
    public void SaveAndClose()
    {
        currentCode.Script = ScriptEditor.Save();

        //string baseType = TypeSignatureManager.Instance.Tree[currentCode.Name];
        //if (TypeSignatureManager.Instance.Tree.Contains(baseType))
        //    TypeSignatureManager.Instance.Tree.Node(baseType).ChildList.Remove(currentCode);
        //currentCode.SetupLayer();
        //baseType = GameEditorManager.Instance.GetBaseType(currentCode);
        //if (TypeSignatureManager.Instance.Tree.Contains(baseType))
        //{
        //    TypeSignatureManager.Instance.Tree[currentCode.Name] = baseType;
        //    TypeSignatureManager.Instance.Tree.Node(baseType).ChildList.Add(currentCode);
        //}

        //ApplyChanges(currentCode);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        ScriptEditor.enabled = false;
#endif
        Visible = false;
    }
    //private void ApplyChanges(CodeFile codeFile)
    //{
    //    string baseType = TypeSignatureManager.Instance.Tree[codeFile.Name];
    //    if (TypeSignatureManager.Instance.Tree.Contains(baseType))
    //        codeFile.ExtendLayer(TypeSignatureManager.Instance.Tree.Node(baseType).CodeFile.Layers);
    //    var data = GameEditorManager.Instance.GetExportParametersData(codeFile);
    //    var node = TypeSignatureManager.Instance.Tree.Node(codeFile.Name);
    //    for (int i = 0; i < node.CodeDataList.Count; i++)
    //    {
    //        GameObjectCodeData codeData = node.CodeDataList[i];
    //        codeData.SetupExportParamData(data);
    //    }
    //    for (int i = 0; i < node.ChildList.Count; i++)
    //    {
    //        var child = node.ChildList[i];
    //        child.SetupLayer();
    //        ApplyChanges(child);
    //    }
    //}

#if UNITY_EDITOR || PLATFORM_STANDALONE_WIN
    public void CopyAllToSystemCopyBuffer()
    {
        currentCode.Script = ScriptEditor.Save();
        codeClipBoard = currentCode.Script;
        GUIUtility.systemCopyBuffer = codeClipBoard;
    }
    public void PasteFromSystemCopyBuffer()
    {
        codeClipBoard = GUIUtility.systemCopyBuffer;
        if (!string.IsNullOrEmpty(codeClipBoard))
            ScriptEditor.Load(codeClipBoard);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.C))
                CopyAllToSystemCopyBuffer();
            else if (Input.GetKeyDown(KeyCode.V))
                PasteFromSystemCopyBuffer();
            else if (Input.GetKeyDown(KeyCode.Delete))
                Clear();
        }
    } 
#endif
}
