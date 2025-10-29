#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FireBulletOnActivate))]
public class FireBulletOnActivateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Disparo", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bullet"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnPoint"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fireSpeed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shotSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldownTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hapticDuration"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hapticAmplitude"));

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Control de disparos", EditorStyles.boldLabel);
        var useLimitProp = serializedObject.FindProperty("useShotLimit");
        EditorGUILayout.PropertyField(useLimitProp);
        if (useLimitProp.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxShots"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shotsScore")); // <-- Aquí el nuevo campo
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Control de tiempo", EditorStyles.boldLabel);
        var useTimerProp = serializedObject.FindProperty("useTimer");
        EditorGUILayout.PropertyField(useTimerProp);
        if (useTimerProp.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gameDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timerText")); // ✅ Mostrar solo si el temporizador está activo
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Final del juego", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("nextSceneName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("endDelay"));

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Sistema de puntos", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pointCounter"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
