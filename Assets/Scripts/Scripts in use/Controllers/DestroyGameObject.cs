using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    void DestroyObject()
    {
        Destroy(this.gameObject.transform.parent.gameObject);
    }
}
