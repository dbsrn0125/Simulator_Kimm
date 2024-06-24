using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public WebSocketManager webSocketManager;
    // Start is called before the first frame update
    void Start()
    {
        webSocketManager = FindObjectOfType<WebSocketManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(webSocketManager != null)
        {
            float spherePositionY = transform.position.y;
            webSocketManager.Send_Message($"SphereY:{spherePositionY}");
        }
    }
}
