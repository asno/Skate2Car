//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(Cinematic))]
//public class CinematicEditor : Editor
//{
//    [MenuItem("Assets/Create/My Cinematic")]
//    public static void CreateMyCinematic()
//    {
//        Cinematic asset = ScriptableObject.CreateInstance<Cinematic>();

//        AssetDatabase.CreateAsset(asset, "Assets/Cinematic.asset");
//        AssetDatabase.SaveAssets();

//        EditorUtility.FocusProjectWindow();

//        Selection.activeObject = asset;
//    }

//    [MenuItem("Assets/Create/My Sequences/My Move To Sequence")]
//    public static void CreateMyMoveToSequence()
//    {
//        SequenceMoveTo asset = ScriptableObject.CreateInstance<SequenceMoveTo>();

//        AssetDatabase.CreateAsset(asset, "Assets/MoveToSequence.asset");
//        AssetDatabase.SaveAssets();

//        EditorUtility.FocusProjectWindow();

//        Selection.activeObject = asset;
//    }

//    [MenuItem("Assets/Create/My Sequences/My Move Horizontal Sequence")]
//    public static void CreateMyMoveHorizontalSequence()
//    {
//        SequenceMoveHorizontal asset = ScriptableObject.CreateInstance<SequenceMoveHorizontal>();

//        AssetDatabase.CreateAsset(asset, "Assets/MoveHorizontalSequence.asset");
//        AssetDatabase.SaveAssets();

//        EditorUtility.FocusProjectWindow();

//        Selection.activeObject = asset;
//    }

//    [MenuItem("Assets/Create/My Sequences/My Teleport Sequence")]
//    public static void CreateMyTeleportSequence()
//    {
//        SequenceTeleport asset = ScriptableObject.CreateInstance<SequenceTeleport>();

//        AssetDatabase.CreateAsset(asset, "Assets/TeleportSequence.asset");
//        AssetDatabase.SaveAssets();

//        EditorUtility.FocusProjectWindow();

//        Selection.activeObject = asset;
//    }
//}
