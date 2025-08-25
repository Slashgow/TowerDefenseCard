using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class CardOutlineHover : MonoBehaviour
{
    [SerializeField] private Color outlineHoverColor = Color.yellow;
    [SerializeField] private SpriteShapeRenderer cardOutline;

    private Color originalOutlineColor;
    private void Awake() => originalOutlineColor = cardOutline.color;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Pointer Entered Card");
        cardOutline.color = outlineHoverColor;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Pointer exited Card");
        cardOutline.color = originalOutlineColor;
    }
}
