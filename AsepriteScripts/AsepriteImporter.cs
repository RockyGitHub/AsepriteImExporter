using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Elendow.SpritedowAnimator;

#if UNITY_EDITOR
public partial class AsepriteImporter : OdinEditorWindow
{
    private static readonly string ASEPRITE_STANDARD_PATH_WINDOWS = @"C:\Program Files\Aseprite\Aseprite.exe";
    private static readonly string ASEPRITE_STANDARD_PATH_MACOSX = @"/Applications/Aseprite.app/Contents/MacOS/aseprite";

    public ScriptableObject SavedSettingsAssetTODO;

    [Sirenix.OdinInspector.Title("Drag and drop works too"), BoxGroup("Exporting spritesheets")]
    [Sirenix.OdinInspector.FilePath(RequireExistingPath = true), BoxGroup("Exporting spritesheets"), OnValueChanged("ValidateExecutable")]
    public string AsepriteExecutable = StandardApplicationPath;

    [Sirenix.OdinInspector.FilePath(RequireExistingPath = true), BoxGroup("Exporting spritesheets"), OnValueChanged("CheckIfAsepriteFile")]
    public string AsepriteFile;

    [Sirenix.OdinInspector.FilePath(RequireExistingPath = true), BoxGroup("Exporting spritesheets"), OnValueChanged("LoadJsonPreview")]
    public string AsepriteJsonSettings;

    [FolderPath(RequireExistingPath = true), BoxGroup("Exporting spritesheets")]
    public string OutputRootPath;

    [SerializeField, BoxGroup("Exporting spritesheets")] private AsepriteExporter.JsonSettings _jsonSettings;
    public static string StandardApplicationPath
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return ASEPRITE_STANDARD_PATH_WINDOWS;
            }
            else
            {
                return ASEPRITE_STANDARD_PATH_MACOSX;
            }
        }
    }

    private bool _running = false; // I was trying to use this to change the button color

    [Button(ButtonSizes.Large), GUIColor("GetExportButtonColor"), BoxGroup("Exporting spritesheets")]
    public void ExportToSpritesheets()
    {
        //lol
        if (GetExportButtonColor() == Color.red)
            return;

        _running = true;
        AsepriteExporter exporter = new AsepriteExporter();
        exporter.Setup(AsepriteJsonSettings, AsepriteFile, OutputRootPath);
        exporter.ExportAllTagsAsSpriteSheetForEachLayer();

        exporter.BlockToFinish();
        AssetDatabase.Refresh();
        _running = false;
    }


    private void CheckIfAsepriteFile()
    {
        if (AsepriteFile.EndsWith(".aseprite") == false)
        {
            Debug.LogError("Not the right type of asset!");
            AsepriteFile = string.Empty;
            return;
        }
    }

    private void LoadJsonPreview()
    {
        TextAsset asset = (TextAsset)AssetDatabase.LoadAssetAtPath(AsepriteJsonSettings, typeof(TextAsset));
        if (asset == null)
        {
            Debug.LogError("Not the right type of asset!");
            AsepriteJsonSettings = string.Empty;
            return;
        }
        _jsonSettings = JsonUtility.FromJson<AsepriteExporter.JsonSettings>(asset.text);
    }

    private void ValidateExecutable()
    {
        if (AsepriteExecutable.EndsWith("Aseprite.exe") == false)
        {
            AsepriteExecutable = string.Empty;
        }
    }

    private Color GetExportButtonColor()
    {
        if (AsepriteExecutable == string.Empty)
            return Color.red;
        if (AsepriteFile == string.Empty)
            return Color.red;
        if (AsepriteJsonSettings == string.Empty)
            return Color.red;
        if (OutputRootPath == string.Empty)
            return Color.red;
        if (_running)
            return Color.yellow;

        return Color.green;
    }

    [MenuItem("Tools/Aseprite/Importer")]
    private static void OpenWindow()
    {
        GetWindow<AsepriteImporter>().Show();
    }

    public enum OutputAsset
    {
        Spritedow,
        Mechanim
    }
}

#endif