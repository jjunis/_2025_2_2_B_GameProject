using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bildboard : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);                //텍스트 뒤집힘 방지
        }
    }
}
