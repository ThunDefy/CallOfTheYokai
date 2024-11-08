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
        currentBoosts += data.GetLevelData(upgradeIndx).boosts;
        return true;
    }

    public List<string> GetBoostsInfo(bool onlyDefault = false)
    {
        FieldInfo[] fields = typeof(PlayerData.Stats).GetFields();
        List<string> boostsInfo = new List<string>(2);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType != typeof(List<int>))
            {
                object value;
                if (!onlyDefault)
                    value = field.GetValue(currentBoosts);
                else value = field.GetValue(data.baseStats.boosts);

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
        // �������������� �������� �� "maxHealth" � "Max Health"
        return Regex.Replace(fieldName, "(\\B[A-Z])", " $1");
    }
}
