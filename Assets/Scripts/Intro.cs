using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class Intro : MonoBehaviour
{
    public GameManager gameManager;
    private Button Introduce;

    void Start()
    {
        Introduce = GetComponent<Button>();
        Introduce.onClick.AddListener(OnButtonClick);
    }
    void OnButtonClick()
    {
        gameManager.StartGame();

        gameObject.SetActive(false);

        gameManager.ResetCamera();

        GameObject mainCanvas = GameObject.Find("Canvas");
        if (mainCanvas != null)
        {
            mainCanvas.SetActive(true);
        }
    }
}
