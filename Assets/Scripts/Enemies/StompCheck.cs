using UnityEngine;

public class StompCheck : MonoBehaviour
{
    public void Die()
    {
        Destroy(transform.parent.gameObject);
    }

}
