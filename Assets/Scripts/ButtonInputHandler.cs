using UnityEngine;
using UnityEngine.EventSystems;// Required when using Event data.

public class ButtonInputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Vector3 moveDir = Vector3.zero;
    [SerializeField] private bool isJump;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private void Start()
    {
        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("buttonPressed");
        if (isJump) 
        {
            playerInputHandler.SubmitJumpRpc();
        }
        else
        {
            playerInputHandler.HorizontalButton(moveDir);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isJump) 
        {
            playerInputHandler.HorizontalButton(-moveDir);
        }
    }

}
