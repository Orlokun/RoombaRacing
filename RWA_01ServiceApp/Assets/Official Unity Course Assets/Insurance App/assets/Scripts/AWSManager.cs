using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;

public class AWSManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);    
    }
}
