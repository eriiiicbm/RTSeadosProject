using UnityEngine;
using System.Collections;

public class DemoReactivator : MonoBehaviour {

	public float TimeDelayToReactivate = 3;
	
	void Start () {
		InvokeRepeating("Reactivate", TimeDelayToReactivate, TimeDelayToReactivate );
	}

	void Reactivate ()
	{
		gameObject.SetActive(false);
		gameObject.SetActive(true);
	}
}
