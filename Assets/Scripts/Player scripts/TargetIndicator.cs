using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{
    private List<Transform> targetObjects = new List<Transform>(); 

    private Transform currentTarget; // ������� ����
    public float minDistance = 1f; // ����������� ���������� ��� �������� ���� �����������
    public Image arrowImg; // ��������� ����

    GameObject boss;
    Portal portal;
    void Update()
    {
        if (currentTarget != null)
        {
            arrowImg.enabled = true;
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position); // ���������� �� ������� ����

            if (distanceToTarget <= minDistance)
            {
                if (targetObjects.Count > 0) 
                {
                    targetObjects.Remove(currentTarget.transform); 
                    FindClosestTarget(); // ������� ��������� ��������� ����
                    //print("���� ����������");
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
                        arrowImg.enabled = false; // ����� ��������� ���������
                }
            }
            else
            {
                UpdateIndicator();// ��������� ���������, ����� �� ������ �������� �� ������� ����
            }
        }
        else
            arrowImg.enabled = false; // ����� ��������� ���������

    }

    public void FindAllTargetObjects()
    {
        targetObjects.Clear();
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("YokaiCage"); 

        foreach (GameObject obj in allObjects)
        {
            targetObjects.Add(obj.transform); // ��������� ������ � ������ �����
        }
    }

    void FindClosestTarget()
    {
        float closestDistance = Mathf.Infinity;
        List<Transform> objectsToRemove = new List<Transform>(); // ������� ������ ��� ���������, ������� ����� �������

        foreach (Transform targetObject in targetObjects)
        {
            if (targetObject != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetObject.position); // ���������� �� ����

                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    currentTarget = targetObject; // ��������� ������� ����
                }
            }
            else
            {
                objectsToRemove.Add(targetObject); // ��������� ������� � ������ ��� ��������
            }
        }

        // ������� �������� ����� ���������� ��������
        foreach (Transform objectToRemove in objectsToRemove)
        {
            targetObjects.Remove(objectToRemove);
        }

        UpdateIndicator();
    }

    void UpdateIndicator()
    {
        // ��������� ����������� � ����
        if (currentTarget != null)
        {
            Vector3 directionToTarget = currentTarget.transform.position - transform.position;
            // ������������ �������, ����� ��� �������� �� ����
            arrowImg.transform.up = directionToTarget.normalized;
        }
    }

    public void StartIndicator()
    {
        portal = FindObjectOfType<Portal>();
        if(portal) portal.transform.gameObject.SetActive(false);
        FindAllTargetObjects(); // ������� ��� �������, ��������������� ���������, ��� �������
        FindClosestTarget(); // ������� ��������� ����
    }
}
