using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sign : Interactable
{
    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    public string dialog;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (Input.GetButtonDown("attack") && playerInRange)
        {
            if (dialogBox.activeInHierarchy)
            {
                dialogBox.SetActive(false);
            }
            else
            {
                dialogBox.SetActive(true);
                dialogText.text = dialog;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            context.Raise();
            playerInRange = false;

            // Kiểm tra xem gameObject vẫn tồn tại trước khi thực hiện bất kỳ hành động nào
            if (gameObject != null)
            {
                dialogBox.SetActive(false);
            }
        }
    }
}