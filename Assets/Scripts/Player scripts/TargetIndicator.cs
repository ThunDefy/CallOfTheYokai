using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{
    private List<Transform> targetObjects = new List<Transform>(); 

    private Transform currentTarget; // текущая цель
    public float minDistance = 1f; // минимальное расстояние для считания цели достигнутой
    public Image arrowImg; // индикатор цели

    GameObject boss;
    Portal portal;
    void Update()
    {
        if (currentTarget != null)
        {
            arrowImg.enabled = true;
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position); // расстояние до текущей цели

            if (distanceToTarget <= minDistance)
            {
                if (targetObjects.Count > 0) 
                {
                    targetObjects.Remove(currentTarget.transform); 
                    FindClosestTarget(); // находим следующую ближайшую цель
                    //print("Цель достигнута");
                }
                else
                {
                    if (boss = GameObject.FindWithTag("Boss"))
                    {
                        currentTarget = boss.transform;
                    }
                    else if (portal)
                    {
                        portal.transform.gameObject.SetActive(true);
                        currentTarget = portal.transform;
                    }
                    else
                        arrowImg.enabled = false; // иначе отключаем индикатор
                }
            }
            else
            {
                UpdateIndicator();// обновляем индикатор, чтобы он всегда указывал на текущую цель
            }
        }
        else
            arrowImg.enabled = false; // иначе отключаем индикатор

    }

    public void FindAllTargetObjects()
    {
        targetObjects.Clear();
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("YokaiCage"); 

        foreach (GameObject obj in allObjects)
        {
            targetObjects.Add(obj.transform); // добавляем объект в список целей
        }
    }

    void FindClosestTarget()
    {
        float closestDistance = Mathf.Infinity;
        List<Transform> objectsToRemove = new List<Transform>(); // создаем список для элементов, которые нужно удалить

        foreach (Transform targetObject in targetObjects)
        {
            if (targetObject != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetObject.position); // расстояние до цели

                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    currentTarget = targetObject; // обновляем текущую цель
                }
            }
            else
            {
                objectsToRemove.Add(targetObject); // добавляем элемент в список для удаления
            }
        }

        // удаляем элементы после завершения перебора
        foreach (Transform objectToRemove in objectsToRemove)
        {
            targetObjects.Remove(objectToRemove);
        }

        UpdateIndicator();
    }

    void UpdateIndicator()
    {
        // Вычисляем направление к цели
        if (currentTarget != null)
        {
            Vector3 directionToTarget = currentTarget.transform.position - transform.position;
            // Поворачиваем стрелку, чтобы она смотрела на цель
            arrowImg.transform.up = directionToTarget.normalized;
        }
    }

    public void StartIndicator()
    {
        portal = FindObjectOfType<Portal>();
        if(portal) portal.transform.gameObject.SetActive(false);
        FindAllTargetObjects(); // находим все объекты, удовлетворяющие критериям, при запуске
        FindClosestTarget(); // находим ближайшую цель
    }
}
