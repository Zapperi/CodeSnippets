//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/**
 * Author: Henri Leppänen
 * <summary>
 * Editor for sticker creation.
 * </summary>
 */

public class CreateStickersTool : EditorWindow
{
    private WorldManager.WorldID world;
    private bool showSprites = true;
    List<Sprite> spriteList = new List<Sprite>();
    Object[] objectList;
    string path;
    GUIStyle style = new GUIStyle();

    private void OnEnable()
    {
        // Position the Text in the center of the Box
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.normal.background = MakeTex(2, 2, new Color(0f, 1f, 0f, 0.5f));
    }

    [MenuItem ("Window/Sticker Creator")]
    static void Init()
    {
        CreateStickersTool window = (CreateStickersTool)GetWindow(typeof(CreateStickersTool));
        window.minSize = new Vector2(300f, 200f);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create Stickers!"))
        {
            CreateStickers();
        }
        world = (WorldManager.WorldID)EditorGUILayout.EnumPopup("Sticker world:", world);
        showSprites = EditorGUILayout.Foldout(showSprites, "Sprites");
        if (showSprites)
        {
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", spriteList.Count));           
            while (newCount < spriteList.Count)
                spriteList.RemoveAt(spriteList.Count - 1);
            while (newCount > spriteList.Count)
                spriteList.Add(null);

            for (int i = 0; i < spriteList.Count; i++)
            {
                spriteList[i] = (Sprite)EditorGUILayout.ObjectField(spriteList[i], typeof(Sprite), allowSceneObjects: true);
            }
        }

        DropAreaGUI();
    }

	// Creates stickers from sprites in their corresponding folder.
    private void CreateStickers()
    {
        path = "Assets/Stickers/" + FirstLetterToUpper(world.ToString());
        int counter = new int();
        foreach(Sprite icon in spriteList)
        {
            Sticker sticker = CreateInstance<Sticker>();
            sticker.name = ("Sticker_" + FirstLetterToUpper(world.ToString()) + "_" + icon.name);
            sticker.icon = icon;
            sticker.world = world;
            sticker.repeatable = false;            
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Stickers", FirstLetterToUpper(world.ToString()));
            }
            var _exists = AssetDatabase.LoadAssetAtPath(path + "/" + sticker.name + ".asset", typeof(Sticker));
            if (_exists != null)
            {
                Debug.LogWarning("Sticker " + sticker.name + " already exists, ignoring it!");               
            }
            else
            {
                AssetDatabase.CreateAsset(sticker, (path + "/" + sticker.name + ".asset"));
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                ++counter;
            }
        }
        Color[] color_debug_log = new Color[]
        {   Color.red,
            Color.green,
            Color.blue,
            Color.magenta};

        if (counter > 0)
        {
            Color color = Color.green;
            Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>",
                (byte)(color.r * 50f), (byte)(color.g * 150f), (byte)(color.b * 50f),
                ("Created (" + counter + ") new stickers to folder at: " + path + "!")));
        }
        spriteList.Clear();


    }
	
    /// <summary>
    /// Converts strings first letter to capital letter.
    /// </summary>
    /// <param name="str">String to convert</param>
    /// <returns>Converted string</returns>
    public string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }
	
    /// <summary>
    /// Creates GUI area to drop sprite images.
    /// </summary>
    public void DropAreaGUI()
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Drag Sprites here!", style);

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    objectList = DragAndDrop.objectReferences;
                    List<Sprite> tempList = new List<Sprite>();
                    bool duplicate;
                    for (int i = 0; i < objectList.Length; i++)
                    {
                        duplicate = false;
                        foreach(Sprite s in spriteList)
                        {
                            if (s.name == objectList[i].name)
                            {
                                Debug.LogWarning("Sprite " +s.name + " already in the list, skipping it!");
                                duplicate = true;
                                continue;
                            }
                        }
                        if (duplicate)
                        {
                            continue;
                        }
                        tempList.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(objectList[i])));                        
                    }
                    spriteList.AddRange(tempList);
                }
                break;
        }
    }
	
	
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
