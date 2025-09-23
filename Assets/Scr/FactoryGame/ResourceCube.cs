using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCube : MonoBehaviour
{
    public ResourceType type;

    public void lnitalize(ResourceType resourceType)
    {
        type = resourceType;
        Renderer renderer = GetComponent<Renderer>();

        if (resourceType == ResourceType.Wood)  renderer.material.color = new Color(0.6f, 0.3f, 0.1f);    //갈색
        if (resourceType == ResourceType.Metal) renderer.material.color = Color.gray;                     //회색  
    }
}
