using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopExitDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("DungeonScene");
        }
    }
}