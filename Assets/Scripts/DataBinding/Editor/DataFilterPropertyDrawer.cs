using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace DataBinding
{
    [CustomPropertyDrawer(typeof(DataSourceFilter))]
    public class DataFilterPropertyDrawer : PropertyDrawer
    {
        private float _entryHeight = 18f;
        private float _indentWidth = 15f;
        private float _entrySpacing = 4f;

        private static readonly Color NewPredicateButtonColor = new Color(0.7f, 0.7f, 1f);
        private static readonly Color NewConditionButtonColor = new Color(0.3f, 1f, 0.3f);
        private static readonly Color RemoveButtonColor = new Color(1f, 0.3f, 0.3f);

        private ConditionTypes _conditionType = ConditionTypes.Bool;

        private float EntryHeightWithSpacing => _entryHeight + _entrySpacing;

        private DataSourceFilter GetPropertyValue(SerializedProperty property)
        {
            var result = (object)property.serializedObject.targetObject;
            var propertyPath = property.propertyPath.Replace(".Array.data", "");

            foreach (var path in propertyPath.Split('.'))
            {
                var destPath = path;
                var openArrayIdx = path.IndexOf('[');
                var closeArrayIdx = path.IndexOf(']');
                var arrayItemIdx = -1;

                if (openArrayIdx < closeArrayIdx)
                {
                    destPath = path.Remove(openArrayIdx);

                    var idxStr = path.Substring(openArrayIdx + 1, closeArrayIdx - openArrayIdx - 1);

                    if (int.TryParse(idxStr, out var idx))
                    {
                        arrayItemIdx = idx;
                    }
                }

                var value = result?.GetType()
                                  .GetField(destPath,
                                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                  ?.GetValue(result);

                result = value is IList valueList && 0 <= arrayItemIdx && arrayItemIdx < valueList.Count
                             ? valueList[arrayItemIdx] : value;
            }

            return result as DataSourceFilter;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertyValue = GetPropertyValue(property);

            if (propertyValue == null)
            {
                return 0f;
            }

            var totalHeight = EntryHeightWithSpacing;

            foreach (var predicate in propertyValue.Predicates)
            {
                totalHeight += GetPredicateHeight(predicate);
            }

            return totalHeight;
        }

        private float GetPredicateHeight(DataSourceFilterPredicate predicate)
        {
            var totalHeight = EntryHeightWithSpacing;

            foreach (var entry in predicate.Entries)
            {
                if (entry.Condition != null)
                {
                    totalHeight += GetPredicateEntryHeight(entry);
                }
            }

            totalHeight += EntryHeightWithSpacing;

            return totalHeight;
        }

        private float GetPredicateEntryHeight(DataSourceFilterPredicateEntry entry)
        {
            return EntryHeightWithSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyValue = GetPropertyValue(property);

            if (propertyValue == null)
            {
                return;
            }

            var originIndent = EditorGUI.indentLevel;

            position = EditorGUI.IndentedRect(position);

            EditorGUI.indentLevel = 0;

            var offsetY = 0f;
            var predicatesToRemove = new HashSet<DataSourceFilterPredicate>();
            var dataFilterLabelRect = new Rect(position.x, position.y + offsetY, position.width / 2f, _entryHeight);

            var newPredicateBtnRect = new Rect(position.x + position.width - position.width / 2f, position.y + offsetY,
                                               position.width / 2f, _entryHeight);

            EditorGUI.LabelField(dataFilterLabelRect, property.displayName);

            StartBgColor(NewPredicateButtonColor);

            {
                if (GUI.Button(newPredicateBtnRect, "New Predicate (OR)", EditorStyles.toolbarButton))
                {
                    propertyValue.Predicates.Add(new DataSourceFilterPredicate());
                }
            }

            EndBgColor();

            offsetY += EntryHeightWithSpacing;

            for (int idx = 0, l = propertyValue.Predicates.Count; idx < l; ++idx)
            {
                var predicate = propertyValue.Predicates[idx];
                var predicateHeight = GetPredicateHeight(predicate);

                var predicatePosition = new Rect(position.x + _indentWidth, position.y + offsetY,
                                                 position.width - _indentWidth, predicateHeight);

                offsetY += predicateHeight;

                DrawPredicate(predicatePosition, predicate, idx, out var needsRemove);

                if (needsRemove)
                {
                    predicatesToRemove.Add(predicate);
                }
            }

            foreach (var predicate in predicatesToRemove)
            {
                propertyValue.Predicates.Remove(predicate);
            }

            if (GUI.changed)
            {
                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUI.indentLevel = originIndent;
        }

        private void DrawPredicate(Rect position, DataSourceFilterPredicate predicate, int predicateIdx,
                                   out bool needsRemove)
        {
            var offsetY = 0f;
            var removeBtnWidth = 32f;
            var entriesToRemove = new HashSet<DataSourceFilterPredicateEntry>();
            var predicateRemoveBtnRect = new Rect(position.x, position.y + offsetY, removeBtnWidth, _entryHeight);

            var predicateLabelRect = new Rect(position.x + removeBtnWidth, position.y + offsetY,
                                              position.width - removeBtnWidth, _entryHeight);

            StartBgColor(RemoveButtonColor);

            {
                needsRemove = GUI.Button(predicateRemoveBtnRect, "DEL", EditorStyles.miniButtonLeft);
            }

            EndBgColor();

            EditorGUI.LabelField(predicateLabelRect, $"Predicate #{predicateIdx}", EditorStyles.miniBoldLabel);

            offsetY += EntryHeightWithSpacing;

            for (int idx = 0, l = predicate.Entries.Count; idx < l; ++idx)
            {
                var entry = predicate.Entries[idx];
                var entryHeight = GetPredicateEntryHeight(entry);
                var entryPosition = new Rect(position.x, position.y + offsetY, position.width, entryHeight);

                offsetY += entryHeight;

                DrawPredicateEntry(entryPosition, entry, idx, out var needsRemoveEntry);

                if (needsRemoveEntry)
                {
                    entriesToRemove.Add(entry);
                }
            }

            foreach (var entry in entriesToRemove)
            {
                predicate.Entries.Remove(entry);
            }

            var addFieldWidth = position.width / 2f;

            var dropConditionTypeRect = new Rect(position.x + position.width - addFieldWidth * 2f, position.y + offsetY,
                                                 addFieldWidth, _entryHeight);

            var newConditionBtnRect = new Rect(position.x + position.width - addFieldWidth, position.y + offsetY,
                                               addFieldWidth, _entryHeight);

            _conditionType =
                (ConditionTypes)EditorGUI.EnumPopup(dropConditionTypeRect, _conditionType,
                                                    EditorStyles.toolbarDropDown);

            StartBgColor(NewConditionButtonColor);

            {
                if (GUI.Button(newConditionBtnRect, "New condition (AND)", EditorStyles.toolbarButton))
                {
                    BaseDataFilterCondition condition;

                    switch (_conditionType)
                    {
                        case ConditionTypes.Bool:
                            condition = new DataFilterConditionBool(default, default, default);
                            break;
                        case ConditionTypes.Integer:
                            condition = new DataFilterConditionInteger(default, default, default);
                            break;
                        case ConditionTypes.Float:
                            condition = new DataFilterConditionFloat(default, default, default);
                            break;
                        case ConditionTypes.String:
                            condition = new DataFilterConditionString(default, default, default);
                            break;
                        case ConditionTypes.HasKey:
                            condition = new DataFilterConditionHasKey(default);
                            break;
                        case ConditionTypes.NotHasKey:
                            condition = new DataFilterConditionNotHasKey(default);
                            break;
                        default:
                            throw new NotImplementedException($"unknown condition type {_conditionType}");
                    }

                    predicate.Entries.Add(new DataSourceFilterPredicateEntry(condition));
                }
            }

            EndBgColor();
        }

        private void DrawPredicateEntry(Rect position, DataSourceFilterPredicateEntry entry, int entryIdx, out bool needsRemove)
        {
            var condition = entry.Condition;
            var offsetY = 0f;
            var removeBtnWidth = 32f;
            var fieldWidth = (position.width - removeBtnWidth) / 3f;
            var rect1 = new Rect(position.x, position.y + offsetY, fieldWidth, _entryHeight);
            var rect2 = new Rect(position.x + fieldWidth, position.y + offsetY, fieldWidth, _entryHeight);
            var rect3 = new Rect(position.x + 2f * fieldWidth, position.y + offsetY, fieldWidth, _entryHeight);

            var rectRemove = new Rect(position.x + position.width - removeBtnWidth, position.y + offsetY,
                                      removeBtnWidth, _entryHeight);

            var fieldStyle = new GUIStyle(GUI.skin.box)
                             {
                                 alignment = TextAnchor.MiddleLeft, padding = new RectOffset(4, 4, 0, 0),
                             };

            if (condition is DataFilterConditionString conditionString)
            {
                condition.Path = EditorGUI.TextField(rect1, condition.Path, fieldStyle);

                conditionString.EqualType =
                    (EqualTypes)EditorGUI.EnumPopup(rect2, conditionString.EqualType, EditorStyles.toolbarDropDown);

                conditionString.Value = EditorGUI.TextField(rect3, conditionString.Value);
            }
            else if (condition is DataFilterConditionInteger conditionInteger)
            {
                condition.Path = EditorGUI.TextField(rect1, condition.Path, fieldStyle);

                conditionInteger.EqualType =
                    (EqualTypes)EditorGUI.EnumPopup(rect2, conditionInteger.EqualType, EditorStyles.toolbarDropDown);

                conditionInteger.Value = EditorGUI.IntField(rect3, conditionInteger.Value);
            }
            else if (condition is DataFilterConditionFloat conditionFloat)
            {
                condition.Path = EditorGUI.TextField(rect1, condition.Path, fieldStyle);

                conditionFloat.EqualType =
                    (EqualTypes)EditorGUI.EnumPopup(rect2, conditionFloat.EqualType, EditorStyles.toolbarDropDown);

                conditionFloat.Value = EditorGUI.FloatField(rect3, conditionFloat.Value);
            }
            else if (condition is DataFilterConditionBool conditionBool)
            {
                condition.Path = EditorGUI.TextField(rect1, condition.Path, fieldStyle);

                conditionBool.EqualType =
                    (EqualTypes)EditorGUI.EnumPopup(rect2, conditionBool.EqualType, EditorStyles.toolbarDropDown);

                conditionBool.Value = EditorGUI.Toggle(rect3, conditionBool.Value);
            }
            else if (condition is DataFilterConditionHasKey)
            {
                condition.Path = EditorGUI.TextField(rect1, condition.Path, fieldStyle);
                EditorGUI.LabelField(rect2, "Exists");
            }
            else if (condition is DataFilterConditionNotHasKey)
            {
                condition.Path = EditorGUI.TextField(rect1, condition.Path, fieldStyle);
                EditorGUI.LabelField(rect2, "Not exists");
            }

            StartBgColor(RemoveButtonColor);

            {
                needsRemove = GUI.Button(rectRemove, "DEL", EditorStyles.miniButtonRight);
            }

            EndBgColor();
        }

        private Color _lastBgColor;

        private void StartBgColor(Color bgColor)
        {
            _lastBgColor = GUI.backgroundColor;
            GUI.backgroundColor = bgColor;
        }

        private void EndBgColor()
        {
            GUI.backgroundColor = _lastBgColor;
        }
    }
}