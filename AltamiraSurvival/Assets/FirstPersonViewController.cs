using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonViewController : MonoBehaviour
{
    [SerializeField]
    private Transform headTransform;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(headTransform.position.x, headTransform.transform.position.y, headTransform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(headTransform.position.x, headTransform.transform.position.y, headTransform.position.z);
    }

    public Transform GetHeadTransform()
    {
        return headTransform;
    }
}
