using NaughtyAttributes;
using UnityEngine;


public class SortOrder : MonoBehaviour
{
    [SerializeField] private bool isCanvas = false;
    [SerializeField, ShowIf("isCanvas")] private Canvas canvas;

    [SerializeField] private bool isSpriteRenderer = false;
    [SerializeField, ShowIf("isSpriteRenderer")] private SpriteRenderer spriteRenderer;

    [SerializeField, Range(-10,300)] private int sortingOrder = 0;

    public int SortingOrder => sortingOrder;
    public Canvas Canvas => canvas;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public bool IsCanvas => isCanvas;
    public bool IsSpriteRenderer => isSpriteRenderer;

}


