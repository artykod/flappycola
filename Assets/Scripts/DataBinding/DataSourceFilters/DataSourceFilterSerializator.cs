using System;
using SimpleJSON;

namespace DataBinding
{
    public static class DataSourceFilterSerializator
    {
        private const string TYPE_NAME = "Type";
        private const string PATH_NAME = "Path";
        private const string VALUE_NAME = "Value";
        private const string EQUAL_TYPE_NAME = "EqualType";

        public static string SerializeCondition(BaseDataFilterCondition condition)
        {
            var json = new JSONObject();

            json[TYPE_NAME] = $"{condition.Type}";
            json[PATH_NAME] = condition.Path;

            switch (condition.Type)
            {
                case ConditionTypes.Bool when condition is DataFilterConditionBool conditionBool:
                {
                    json[VALUE_NAME] = conditionBool.Value;
                    json[EQUAL_TYPE_NAME] = $"{conditionBool.EqualType}";

                    break;
                }
                case ConditionTypes.Float when condition is DataFilterConditionFloat conditionFloat:
                {
                    json[VALUE_NAME] = conditionFloat.Value;
                    json[EQUAL_TYPE_NAME] = $"{conditionFloat.EqualType}";

                    break;
                }
                case ConditionTypes.Integer when condition is DataFilterConditionInteger conditionInteger:
                {
                    json[VALUE_NAME] = conditionInteger.Value;
                    json[EQUAL_TYPE_NAME] = $"{conditionInteger.EqualType}";

                    break;
                }
                case ConditionTypes.String when condition is DataFilterConditionString conditionString:
                {
                    json[VALUE_NAME] = conditionString.Value;
                    json[EQUAL_TYPE_NAME] = $"{conditionString.EqualType}";

                    break;
                }
                case ConditionTypes.HasKey when condition is DataFilterConditionHasKey:
                case ConditionTypes.NotHasKey when condition is DataFilterConditionNotHasKey:
                {
                    break;// no specific data to serialize
                }
                default:
                {
                    throw new NotImplementedException($"unknown condition type {condition.Type}");
                }
            }

            return json.ToString();
        }

        public static BaseDataFilterCondition DeserializeCondition(string conditionData)
        {
            var json = JSON.Parse(conditionData);
            var conditionType = ParseEnum<ConditionTypes>(json, TYPE_NAME);
            var conditionPath = json[PATH_NAME].Value;

            switch (conditionType)
            {
                case ConditionTypes.Bool:
                {
                    var value = json[VALUE_NAME].AsBool;
                    var equalType = ParseEnum<EqualTypes>(json, EQUAL_TYPE_NAME);

                    return new DataFilterConditionBool(conditionPath, value, equalType);
                }
                case ConditionTypes.Float:
                {
                    var value = json[VALUE_NAME].AsFloat;
                    var equalType = ParseEnum<EqualTypes>(json, EQUAL_TYPE_NAME);

                    return new DataFilterConditionFloat(conditionPath, value, equalType);
                }
                case ConditionTypes.Integer:
                {
                    var value = json[VALUE_NAME].AsInt;
                    var equalType = ParseEnum<EqualTypes>(json, EQUAL_TYPE_NAME);

                    return new DataFilterConditionInteger(conditionPath, value, equalType);
                }
                case ConditionTypes.String:
                {
                    var value = json[VALUE_NAME].Value;
                    var equalType = ParseEnum<EqualTypes>(json, EQUAL_TYPE_NAME);

                    return new DataFilterConditionString(conditionPath, value, equalType);
                }
                case ConditionTypes.HasKey:
                {
                    return new DataFilterConditionHasKey(conditionPath);
                }
                case ConditionTypes.NotHasKey:
                {
                    return new DataFilterConditionNotHasKey(conditionPath);
                }
                default:
                {
                    throw new NotImplementedException($"unknown condition type {conditionType}");
                }
            }
        }

        private static T ParseEnum<T>(JSONNode json, string name)
        {
            return (T)Enum.Parse(typeof(T), json[name].Value);
        }
    }
}