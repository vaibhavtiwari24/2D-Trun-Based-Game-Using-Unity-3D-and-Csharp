using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSelector : MonoBehaviour
{
    void Update()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Camera.main is not set in the scene!");
            return;
        }

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);
    }
}
