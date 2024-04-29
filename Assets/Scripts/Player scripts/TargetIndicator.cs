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

    //private void Start()
    //{
    //    portal = FindObjectOfType<Portal>();
    //}
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
                    print("���� ����������");
                }
                else
                {
                    if(boss = GameObject.FindWithTag("Boss"))
                    {
                        currentTarget = boss.transform;
                    }else if(portal = FindObjectOfType<Portal>()) currentTarget = portal.transform;
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
            print(obj.name);
            targetObjects.Add(obj.transform); // ��������� ������ � ������ �����
        }
    }

    void FindClosestTarget()
    {
        float closestDistance = Mathf.Infinity; 

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
            }else targetObjects.Remove(targetObject);
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
        FindAllTargetObjects(); // ������� ��� �������, ��������������� ���������, ��� �������
        FindClosestTarget(); // ������� ��������� ����
    }
}
