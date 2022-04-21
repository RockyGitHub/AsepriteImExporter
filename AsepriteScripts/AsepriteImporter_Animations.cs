using Elendow.SpritedowAnimator;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public partial class AsepriteImporter
{
    [EnumPaging, BoxGroup("Importing Animations"), OnValueChanged("NotImplementedWarning")]
    public OutputAsset OutputType;

    [FolderPath(RequireExistingPath = true), BoxGroup("Importing Animations")]
    public string InputRootPath;

    [FolderPath(RequireExistingPath =true), BoxGroup("Importing Animations")]
    public string OutputRootPathAnims;

    [Button(ButtonSizes.Large), GUIColor("GetImportButtonColor"), BoxGroup("Importing Animations")]
    public void ImportSheetsToAnimations()
    {
        //lol
        if (GetImportButtonColor() == Color.red)
            return;

        // Foreach directory at InputRootPath
        //      create the same directory starting at the OutputRootPathAnims directory
        /*        string[] subDirectories;
                string[] subFiles;
                subDirectories = System.IO.Directory.GetDirectories(InputRootPath);
                subFiles = System.IO.Directory.GetFiles(InputRootPath);
                foreach (var dir in subDirectories)
                {
                    Debug.Log(dir);
                }*/
        List<DirectoryFilePair> pairs = new List<DirectoryFilePair>();
        DirSearch(InputRootPath, pairs);


 /*       foreach (var item in pairs)
        {
            item.Print();
        }*/


        foreach (var pair in pairs)
        {
            //Directory.CreateDirectory("Assets/Scripts/Editor/" + cmd.exportPath);
            foreach (var spritesheetPath in pair.Files)
            {
                Object[] spriteSheet = AssetDatabase.LoadAllAssetsAtPath(spritesheetPath);
                var sprites = spriteSheet.Where(q => q is Sprite).Cast<Sprite>().ToList();
                SpriteAnimation animation = CreateInstance<SpriteAnimation>();
                animation.FPS = 10;
                animation.Frames = sprites;

                //AssetDatabase.CreateAsset(animation, OutputRootPathAnims + "/testing.asset");
                AssetDatabase.CreateAsset(animation, OutputRootPathAnims + "/" + Path.GetFileName(spritesheetPath) + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                //EditorUtility.FocusProjectWindow();
            }
        }
    }

    private void DirSearch(string dir, List<DirectoryFilePair> pairs)
    {
        try
        {
            string[] files = Directory.GetFiles(dir, "*.png");
            DirectoryFilePair pair = new DirectoryFilePair(dir, files);
            pairs.Add(pair);

            foreach (string d in Directory.GetDirectories(dir))
                DirSearch(d, pairs);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private Color GetImportButtonColor()
    {
        return Color.white;
    }

    private void NotImplementedWarning()
    {
        if (OutputType == OutputAsset.Mechanim)
            Debug.LogWarning("This setting is not implemented yet: [" + OutputType.ToString() + "]");
    }


    private class DirectoryFilePair
    {
        public DirectoryFilePair(string directoryName, string[] fileNames)
        {
            Directory = directoryName;
            Files = fileNames;
        }

        public string Directory;
        public string[] Files;

        public void Print()
        {
            Debug.Log("Directory: " + Directory);
            foreach (var file in Files)
                Debug.Log("File: "+ file);
        }
    }
}

#endif