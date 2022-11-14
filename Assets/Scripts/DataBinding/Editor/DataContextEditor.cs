using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DataBinding
{
    [CustomEditor(typeof(DataContext))]
    public class DataContextEditor : UnityEditor.Editor
    {
        private static readonly Dictionary<IDataSource, bool> FoldingStates = new Dictionary<IDataSource, bool>();

        public override void OnInspectorGUI()
        {
            var targetDataSource = (target as DataContext)?.DataSource;

            if (targetDataSource == null)
            {
                EditorGUILayout.LabelField("Data available at runtime only");

                return;
            }

            var foldoutStyle = new GUIStyle(EditorStyles.foldout) {fontStyle = FontStyle.Bold};

            var sharedDataStyle = new GUIStyle(EditorStyles.label) {fontStyle = FontStyle.Italic};

            EditorGUI.indentLevel = 1;

            EditorGUILayout.Space();

            DrawDataSource(targetDataSource);

            EditorGUILayout.Space();

            void DrawDataSource(IDataSource dataSource)
            {
                FoldingStates.TryGetValue(dataSource, out var foldingState);

                FoldingStates[dataSource] = EditorGUILayout.Foldout(foldingState, dataSource.Name, true, foldoutStyle);

                if (!foldingState)
                {
                    return;
                }

                EditorGUI.indentLevel++;

                dataSource.ForEach<IDataNode>(DrawDataProperty);

                EditorGUI.indentLevel--;
            }

            void DrawDataProperty(IDataNode property)
            {
                if (property == null)
                {
                    return;
                }

                if (property is IDataProperty dataProperty)
                {
                    EditorGUILayout.LabelField(
                        $"{property.Name}: {property.GetType().Name.Replace("Property", "")}",
                        dataProperty.GetValue<string>());
                }

                if (property is ICommandProperty)
                {
                    EditorGUILayout.LabelField($"{property.Name}: Command");
                }

                if (property is IDataSource nestedDataSource)
                {
                    DrawDataSource(nestedDataSource);
                }
            }
        }
    }
}