using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoseRandomSprite : MonoBehaviour
{
    public List<Sprite> spriteList; // Список доступных спрайтов
    public SpriteRenderer spriteRenderer; // Ссылка на компонент SpriteRenderer

    void Start()
    {
        // Выбор случайного индекса из списка спрайтов
        int randomIndex = Random.Range(0, spriteList.Count);

        // Установка выбранного спрайта на SpriteRenderer
        spriteRenderer.sprite = spriteList[randomIndex];
    }
}
