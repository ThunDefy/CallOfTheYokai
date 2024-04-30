using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class Passive : Yokai
{
    public PassiveData data; 
    [SerializeField] PlayerData.Stats currentBoosts;

    [System.Serializable]

    public struct Modifier
    {
        public string name, description;
        public PlayerData.Stats boosts;
    }
    protected virtual void Awake()
    {
        if (data)
        {
            Initialise(data);
        }
    }

    public virtual void Initialise(PassiveData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    public virtual PlayerData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    public override bool DoLevelUp(int upgradeIndx)
    {
        //if (!CanLevelUp())
        //{
        //    return false;
        //}
        currentBoosts += data.GetLevelData(upgradeIndx).boosts;
        return true;
    }

    public List<string> GetBoostsInfo()
    {
        FieldInfo[] fields = typeof(PlayerData.Stats).GetFields();
        List<string> boostsInfo = new List<string>(2);

        foreach (FieldInfo field in fields)
        {
            //value is int ? (int)value : (float)value;
            //float value = (float)field.GetValue(currentBoosts); // Получаем значение поля
            if (field.FieldType != typeof(List<int>))
            {
                object value = field.GetValue(currentBoosts);
                float fval = value is int ? (int)value : (float)value;

                if (fval != 0)
                {
                    string formattedFieldName = FormatFieldName(field.Name);
                    boostsInfo.Add($"{formattedFieldName}: {value}");
                }
            }
        }
        return boostsInfo;
    }

    private string FormatFieldName(string fieldName)
    {
        // Преобразование например из "maxHealth" в "Max Health"
        return Regex.Replace(fieldName, "(\\B[A-Z])", " $1");
    }
}
