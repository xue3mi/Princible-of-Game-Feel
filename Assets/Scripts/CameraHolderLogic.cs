using UnityEngine;

public class CameraHolderLogic : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float lerpSpeed;

    private void LateUpdate()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, player.position, lerpSpeed * Time.deltaTime);
        //camera keep distance "z" from player so it wont overlap
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }
}
