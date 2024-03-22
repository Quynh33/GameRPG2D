using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeSceneOnCollision : MonoBehaviour
{
    public float delayTime = 2f;

    private void OnTriggerEnter2D (Collider2D other)
    {
        // Kiểm tra xem collider va chạm có phải là người chơi không
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            // Bắt đầu Coroutine để chờ đợi trước khi chuyển cảnh
            StartCoroutine(DelayedSceneChange());
        }
    }

    IEnumerator DelayedSceneChange()
    {
        // Đợi trong khoảng thời gian được thiết lập
        yield return new WaitForSeconds(delayTime);

        // Chuyển cảnh sau khi đợi
        SceneManager.LoadScene("Endgame");
    }
}
