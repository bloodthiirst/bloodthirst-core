using UnityEditor;

public interface IEditorTask<TEditor> where TEditor : EditorWindow
{
    void Execute(TEditor editorWindow);
}