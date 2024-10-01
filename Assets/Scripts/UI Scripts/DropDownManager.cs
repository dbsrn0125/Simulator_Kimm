using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEditor;

public class DropDownManager : MonoBehaviour
{
    public TMP_Dropdown mapDropdown;

    private List<string> mapPaths = new List<string>();

    string currentMap;

    void Start()
    {
        UpdateDropdown();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnSceneChanged(Scene prevScene, Scene newScene)
    {
        string currentMap = newScene.name;
        print(currentMap);
    }
    void UpdateDropdown()
    {
        mapDropdown.ClearOptions();
        mapPaths.Clear();

        List<string> options = new List<string>();

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                string scenePath = scene.path;
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                mapPaths.Add(scenePath);
                options.Add(sceneName);
            }
        }

        mapDropdown.AddOptions(options);
        mapDropdown.onValueChanged.AddListener(delegate { LoadSelectedMap(mapDropdown.value); });
    }

    void LoadSelectedMap(int index)
    {
        SceneManager.LoadScene(mapPaths[index]);
    }
}
