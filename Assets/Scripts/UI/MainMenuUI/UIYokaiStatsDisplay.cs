using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Reflection;
using JetBrains.Annotations;

public class UIYokaiStatsDisplay : MonoBehaviour
{
    public PlayerWeaponData yokaiData;
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

        if (!yokaiData) return;

        if(!statNames) statNames = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (!statValues) statValues = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();

        // ������� ��� ��������� ���� ���������� (����� ����������� (���� ����� ����) )
        FieldInfo[] fields = typeof(Weapon.Stats ).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            
            if (field.FieldType == typeof(float) || field.FieldType == typeof(int))
            {
                names.AppendLine(field.Name);
                object value = field.GetValue(yokaiData.baseStats);
                float fval = value is int ? (int)value : (float)value;
                if(fval == -1)
                {
                    values.Append("-").Append("\n");
                }else values.Append(fval).Append("\n");


                statNames.text = UIStatsDisplay.PrettifyNames(names);
                statValues.text = values.ToString();
            }
        }

        FieldInfo[] passivefields = typeof(PlayerData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in passivefields)
        {
            if (field.FieldType == typeof(float) || field.FieldType == typeof(int))
            {
                
                object value = field.GetValue(yokaiData.passiveEffectData.baseStats.boosts);
                float fval = value is int ? (int)value : (float)value;
                if(fval != 0) {
                    names.AppendLine("Passive: "+field.Name);
                    if (fval == -1)
                    {
                        values.Append("-").Append("\n");
                    }
                    else values.Append(fval).Append("\n");
                }
                statNames.text = UIStatsDisplay.PrettifyNames(names);
                statValues.text = values.ToString();

            }
        }


    }

}
