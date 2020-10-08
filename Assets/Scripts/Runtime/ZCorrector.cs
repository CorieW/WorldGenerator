using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ZCorrector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y /*- (1.75f * (transform.localScale.y-1))*/);
    }
}
