using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class MapManagerWindow : EditorWindow
{
    [MenuItem("Window/Map Manager")]
    public static void ShowWindow()
    {
        GetWindow<MapManagerWindow>("Map Manager");
    }

    private string packagePath = "";
    private Vector2 scrollPos;

    void OnGUI()
    {
        GUILayout.Label("Add & Delete Map", EditorStyles.boldLabel);

        // 맵 추가 섹션
        GUILayout.Label("Add Map", EditorStyles.label);

        if (GUILayout.Button("Select Package File"))
        {
            packagePath = EditorUtility.OpenFilePanel("Select Package File", "", "unitypackage");
        }

        GUILayout.Label("Selected Path: " + packagePath);

        if (!string.IsNullOrEmpty(packagePath) && GUILayout.Button("Add Map"))
        {
            ImportPackageAndAddScenes(packagePath);
            packagePath = ""; // 패키지 경로 초기화
        }

        // 맵 제거 섹션
        GUILayout.Label("Map List", EditorStyles.label);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));

        var existingScenes = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                existingScenes.Add(scene.path);
            }
        }

        foreach (var scenePath in existingScenes)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(scenePath, GUILayout.Width(200));
            if (GUILayout.Button("Delete", GUILayout.Width(50)))
            {
                RemoveMap(scenePath);
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    async void ImportPackageAndAddScenes(string path)
    {
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            AssetDatabase.ImportPackage(path, false);
            AssetDatabase.Refresh();
            await Task.Delay(1000);  // 패키지 임포트가 완료될 때까지 잠시 대기

            AddScenesInAssetsScenesFolder();
        }
        else
        {
            EditorUtility.DisplayDialog("오류", "유효한 패키지 파일 경로를 입력하세요.", "확인");
        }
    }

    void AddScenesInAssetsScenesFolder()
    {
        var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        string[] scenePaths = Directory.GetFiles("Assets/Scenes", "*.unity", SearchOption.AllDirectories);

        foreach (var scenePath in scenePaths)
        {
            string fullScenePath = Path.GetFullPath(scenePath);
            string dataPath = Path.GetFullPath(Application.dataPath);

            if (fullScenePath.StartsWith(dataPath))
            {
                string relativePath = "Assets" + fullScenePath.Substring(dataPath.Length).Replace("\\", "/");

                if (!scenes.Exists(scene => scene.path == relativePath))
                {
                    scenes.Add(new EditorBuildSettingsScene(relativePath, true));
                    Debug.Log("Scene added to build settings: " + relativePath);
                }
            }
            else
            {
                Debug.LogWarning("Scene path does not start with Application.dataPath: " + fullScenePath);
            }
        }

        EditorBuildSettings.scenes = scenes.ToArray();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("성공", "맵이 성공적으로 추가되었습니다.", "확인");
    }

    void RemoveMap(string path)
    {
        // 빌드 세팅에서 씬 제거
        var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        scenes.RemoveAll(scene => scene.path == path);
        EditorBuildSettings.scenes = scenes.ToArray();

        // 실제 파일 시스템에서 씬 파일 삭제
        if (AssetDatabase.DeleteAsset(path))
        {
            Debug.Log("Scene file deleted: " + path);
        }
        else
        {
            Debug.LogError("Failed to delete scene file: " + path);
        }

        // 에셋 데이터베이스 저장 및 새로고침
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("성공", "맵이 성공적으로 제거되었습니다.", "확인");
    }

}
