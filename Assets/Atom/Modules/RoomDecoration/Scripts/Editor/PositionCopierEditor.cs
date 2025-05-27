using UnityEngine;
using UnityEditor;
using RoomDecoration;

[CustomEditor(typeof(PlaceFurnitureButton))]
public class PositionCopierEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlaceFurnitureButton positionCopier = (PlaceFurnitureButton)target;

        if (GUILayout.Button("Copy Position"))
        {
            //positionCopier.CopyPosition();
        }

        if (GUILayout.Button("Paste Position"))
        {
            //positionCopier.PastePosition();
        }
    }
}
