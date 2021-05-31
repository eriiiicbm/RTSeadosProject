using UnityEngine;
using System.Collections;

public class LightCurves : MonoBehaviour {
	public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public float GraphScaleX = 1, GraphScaleY = 1;

	private float startTime;
	private Light lightSource;

	void Start(){
		lightSource = GetComponent<Light> ();
	}

	void OnEnable () {
		startTime = Time.time;
	}
	
	void Update () {
		var time = Time.time - startTime;
		if (time  <= GraphScaleX)
		{
			var eval = LightCurve.Evaluate (time / GraphScaleX) * GraphScaleY;
			lightSource.intensity = eval;
		}
	}
}
