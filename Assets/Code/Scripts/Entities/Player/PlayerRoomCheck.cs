using UnityEngine;

public class PlayerRoomCheck : MonoBehaviour
{
    public CameraFollow cameraFollow;

    public GameObject nextRoomPrefab;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Room"))
        {
            cameraFollow.CurrentRoom = other.gameObject;
            cameraFollow.playerFocus = false;
        } 
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Room"))
        {
            cameraFollow.playerFocus = true;
        }
    }


}
