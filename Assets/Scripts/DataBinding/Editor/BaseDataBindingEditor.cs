using UnityEditor;
using UnityEngine;

namespace DataBinding
{
    [CustomEditor(typeof(BaseDataBinding), true)]
    public class BaseDataBindingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (target is BaseDataBinding dataBinding)
            {
                var go = dataBinding.gameObject;
                var dataContext = dataBinding.DataContext;

                if (dataContext == null)
                {
                    dataContext = go.GetComponent<DataContext>() ?? go.GetComponentInParent<DataContext>();
                }

                if (dataContext != null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Data Context", dataContext, target.GetType(), true);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    var style = new GUIStyle(GUI.skin.label) {richText = true};
                    EditorGUILayout.LabelField("<color=red><b>Data Context not found!</b></color>", style);
                }
            }

            base.OnInspectorGUI();
        }
    }
}