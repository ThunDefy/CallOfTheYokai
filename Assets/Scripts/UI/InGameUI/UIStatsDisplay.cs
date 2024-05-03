using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Reflection;
using JetBrains.Annotations;

public class UIStatsDisplay : MonoBehaviour
{
    public PlayerStats player;
    TextMeshProUGUI statNames, statValues;

    public bool updateInEditor = false;

    private void OnEnable()
    {
        UpdateStatFields();
    }

    private void OnDrawGizmosSelected()
    {
        if(updateInEditor)UpdateStatFields();
    }

    public void UpdateStatFields()
    {

        if (!player) return;

        if(!statNames) statNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!statValues) statValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();

        // Получаю все доступные поля статистики (кроме статических (если будут кнчн) )
        FieldInfo[] fields = typeof(PlayerData.Stats ).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType != typeof(List<int>))
            {
                if (field.Name == "availableSlots" || field.Name == "startingYokaiID") continue;

                names.AppendLine(field.Name);
                object value = field.GetValue(player.actualStats);
                float fval = value is int ? (int)value : (float)value; 

                //Припишем % если значение имеет атрибут и оно float

                PropertyAttribute attribure = (PropertyAttribute)PropertyAttribute.GetCustomAttribute(field, typeof(PropertyAttribute));
                if (attribure != null && field.FieldType == typeof(float))
                {
                    float percentage = Mathf.Round(fval * 100 - 100);
                    if (Mathf.Approximately(percentage, 0))
                    {
                        values.Append("-").Append("\n");
                    }
                    else
                    {
                        if (percentage > 0) values.Append("+");

                        values.Append(percentage).Append("%").Append("\n");
                    }
                }
                else
                {
                    values.Append(fval).Append("\n");
                }

                statNames.text = PrettifyNames(names);
                statValues.text = values.ToString();
            }
        }

    }

    public static string PrettifyNames(StringBuilder input)
    {
        if(input.Length <=0) return string.Empty;

        StringBuilder result = new StringBuilder();
        char last = '\0';
        for(int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if(last == '\0' || char.IsWhiteSpace(last))
            {
                c = char.ToUpper(c);
            }else if (char.IsUpper(c))
            {
                result.Append(' ');
            }
            result.Append(c);

            last = c;  
        }
        return result.ToString();
    }

    private void Reset()
    {
        player = FindAnyObjectByType<PlayerStats>();
    }

}
