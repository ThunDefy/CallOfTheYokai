using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoseRandomSprite : MonoBehaviour
{
    public List<Sprite> spriteList; // ������ ��������� ��������
    public SpriteRenderer spriteRenderer; // ������ �� ��������� SpriteRenderer

    void Start()
    {
        // ����� ���������� ������� �� ������ ��������
        int randomIndex = Random.Range(0, spriteList.Count);

        // ��������� ���������� ������� �� SpriteRenderer
        spriteRenderer.sprite = spriteList[randomIndex];
    }
}
