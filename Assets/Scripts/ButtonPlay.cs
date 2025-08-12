using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class ButtonPlay : MonoBehaviour
{
    public GameManager gameManager; 
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

    }

    void OnButtonClick()
    {
        gameManager.IntroGame();
    }
}
