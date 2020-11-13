using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kimiguna.Graphics2D;

namespace KimigunaEditor.Graphics2D
{
    [CustomEditor(typeof(PixelPerfectCameraAdjustment))]
    public class PixelPerfectCameraAdjustmentEditor : Editor
    {
        #region Fields
            SerializedProperty m_matchMode;
            SerializedProperty m_matchRatio;
        #endregion

        #region Event
            private void OnEnable() {
                m_matchMode = serializedObject.FindProperty(nameof(m_matchMode));
                m_matchRatio = serializedObject.FindProperty(nameof(m_matchRatio));
            }

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                if( m_matchMode.intValue==(int)PixelPerfectCameraAdjustment.MatchMode.WidthOrHeight )
                using(var ind = new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_matchRatio);
                    
                    var rc = EditorGUILayout.GetControlRect();

                    rc.y -= EditorGUIUtility.singleLineHeight*0.5f;
                    rc.xMin += EditorGUIUtility.labelWidth;
                    GUI.Label(new Rect(rc.x,rc.y,50f,rc.height), "Width");
                    rc.xMin = rc.xMax-100f;
                    GUI.Label(new Rect(rc.x,rc.y,50f,rc.height), "Height");
                }
                serializedObject.ApplyModifiedProperties();

                var cmp = (PixelPerfectCameraAdjustment)target;
                if(Application.isPlaying)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox($"Computed Match Ratio : {cmp.computedMatchRatio}\nRaciprocal : {1f/Camera.main.aspect}", MessageType.Info);
                }
            }
        #endregion
    }
}