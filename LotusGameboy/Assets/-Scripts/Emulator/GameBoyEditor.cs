#if UNITY_EDITOR
using Lotus.GameboyEmulator;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameBoy))]
public class GameBoyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Export Binary"))
        {
            GameBoy gb = (target as GameBoy);
            gb.RequestStep();
        }
    }
}
#endif