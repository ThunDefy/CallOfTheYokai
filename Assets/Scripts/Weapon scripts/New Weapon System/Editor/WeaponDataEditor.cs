using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

[CustomEditor(typeof(PlayerWeaponData))]
public class WeaponDataEditor : Editor
{
    PlayerWeaponData weaponData;
    string[] weaponSubtypes;
    int selectedWeaponSubtype;

    private void OnEnable()
    {
        weaponData = (PlayerWeaponData)target;

        // »щем все подклассы Weapon
        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType).ToList();

        List<string> subTypesStr = subTypes.Select(t=>t.Name).ToList();
        subTypesStr.Insert(0, "None");
        weaponSubtypes = subTypesStr.ToArray();

        selectedWeaponSubtype = Math.Max(0,Array.IndexOf(weaponSubtypes,weaponData.behaviour));
    }

    public override void OnInspectorGUI()
    {
        selectedWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectedWeaponSubtype), weaponSubtypes);

        if(selectedWeaponSubtype > 0)
        {
            weaponData.behaviour = weaponSubtypes[selectedWeaponSubtype].ToString();
            EditorUtility.SetDirty(weaponData);
            DrawDefaultInspector();
        }
    }
}
