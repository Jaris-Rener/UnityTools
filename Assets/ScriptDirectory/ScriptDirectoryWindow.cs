using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScriptDirectoryWindow
    : EditorWindow
{
    private const string _scriptDirectoryPath = "ScriptDirectoryPath";
    private string _search;

    [MenuItem("Tools/Scripts Directory")]
    public static void Init()
    {
        _window = (ScriptDirectoryWindow) GetWindow(typeof(ScriptDirectoryWindow));
        _window.titleContent = new GUIContent("Script Directory", EditorGUIUtility.FindTexture("d__Help"));
        _window.Show();
    }

    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        var label = new GUIStyle(EditorStyles.boldLabel);
        label.richText = true;
        GUILayout.Label($"Scripts Directory — <size=9><color=#777>{_directory}</color></size>", label);

        var btnStyle = new GUIStyle("IconButton");
        btnStyle.fixedHeight = EditorGUIUtility.singleLineHeight;
        btnStyle.margin.top = 4;

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_CreateAddNew"), btnStyle))
            AddFile();

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh"), btnStyle) || _files == null)
            RefreshList();

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_FolderOpened Icon"), btnStyle))
            SetPath();

        GUILayout.Space(4);

        GUILayout.EndHorizontal();
        _search = EditorGUILayout.TextField(_search, (GUIStyle) "SearchTextField");
        GUILayout.Space(2);

        var scrollViewStyle = new GUIStyle("ProfilerScrollviewBackground");
        scrollViewStyle.margin = new RectOffset(4, 4, 0, 4);
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, scrollViewStyle);
        if (_files != null)
        {
            var filteredFiles = string.IsNullOrWhiteSpace(_search)
                ? _files
                : _files.Where(x => x.FullName.Contains(_search));
            foreach (var file in filteredFiles)
            {
                DrawFile(file);
            }
        }

        GUILayout.EndScrollView();
    }

    private void SetPath()
    {
        var dir = EditorUtility.OpenFolderPanel("Choose script directory path: ",
            EditorPrefs.GetString(_scriptDirectoryPath, Application.dataPath), string.Empty);

        if (string.IsNullOrWhiteSpace(dir))
            return;

        if (!Directory.Exists(dir))
        {
            Debug.Log($"Could not locate valid directory: {dir}");
            return;
        }

        EditorPrefs.SetString(_scriptDirectoryPath, dir);
        _directory = dir;
        RefreshList();
    }

    private void DrawFile(FileInfo file)
    {
        using (new EditorGUI.DisabledScope(File.Exists(Application.dataPath + $"/{file.Name}")))
        {
            var bgStyle = new GUIStyle("RL FooterButton");
            bgStyle.fixedHeight = 24;
            bgStyle.stretchHeight = true;
            bgStyle.stretchWidth = true;
            bgStyle.border.bottom = 2;

            var pathStyle = new GUIStyle("Label");
            pathStyle.fontSize = 10;
            pathStyle.normal.textColor = Color.grey;

            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Label(EditorGUIUtility.IconContent("d_cs Script Icon"), GUILayout.Width(18), GUILayout.Height(18));
            GUILayout.Label(file.Name, GUILayout.MaxWidth(Mathf.Min(128, position.width - 75)));
            GUILayout.Label(file.FullName, pathStyle, GUILayout.MaxWidth(position.width - 180));
            GUILayout.FlexibleSpace();

            var importBtnStyle = new GUIStyle("IconButton");
            importBtnStyle.margin.top = 4;
            if (GUILayout.Button(EditorGUIUtility.IconContent("winbtn_win_close"), importBtnStyle))
            {
                PromptDeleteFile(file);
            }
            if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), importBtnStyle))
            {
                ImportFile(file);
            }
            GUILayout.EndHorizontal();
        }
    }

    private void PromptDeleteFile(FileInfo file)
    {
        if (EditorUtility.DisplayDialog("Permanently Delete Script", $"Are you sure you want to delete {file} from your Script Directory? This action can not be reversed.",
            "Delete", "Cancel"))
        {
            File.Delete(file.FullName);
            RefreshList();
        }
    }

    private void AddFile()
    {
        if (Selection.objects.Length > 1 || !(Selection.activeObject is TextAsset asset))
            return;

        var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, path);
        var ext = Path.GetExtension(path);
        if (ext != ".cs")
            return;

        var targetPath = EditorPrefs.GetString(_scriptDirectoryPath, Application.dataPath) + $"/{Selection.activeObject.name}.cs";
        if (File.Exists(targetPath))
            return;

        if (EditorUtility.DisplayDialog("Add Script", $"Do you want to add {asset.name} to your Script Directory?",
            "Add", "Cancel"))
        {
            File.Copy(path, targetPath);
            RefreshList();
        }
    }

    private void ImportFile(FileInfo file)
    {
        if (EditorUtility.DisplayDialog("Add Script",
            $"Do you want to add {file.Name} to the project?",
            "Import", "Cancel"))
        {
            var targetPath = Application.dataPath + $"/{file.Name}";
            if (File.Exists(targetPath))
            {
                Debug.Log("File already exists in project.");
                return;
            }

            File.Copy(file.FullName, targetPath);
            AssetDatabase.Refresh();
        }
    }

    private FileInfo[] _files;
    private Vector2 _scrollPos;
    private static ScriptDirectoryWindow _window;
    private string _directory;

    private void RefreshList()
    {
        var path = EditorPrefs.GetString(_scriptDirectoryPath, string.Empty);
        if (!Directory.Exists(path))
            return;

        var directory = new DirectoryInfo(path);
        _files = directory.GetFiles("*.cs", SearchOption.AllDirectories);
        _directory = path;
    }
}