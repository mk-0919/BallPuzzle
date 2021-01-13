using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject gameOverPanel;
    public bool isGameOver;
    void OnTriggerEnter2D(Collider2D c)
    {

        if(c.gameObject.tag == "Ball")
        {
            gameOverPanel.SetActive(true);
            isGameOver = true;
        }
    }
}
