using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsatingLight : MonoBehaviour
{
	[SerializeField]
	private bool HitMaxIntensity;
	[SerializeField]
	private float additiveIntensity = 1f;
	public float MaxIntensity = 130f;
	public float MinIntensty = 100f;

	// Update is called once per frame
	void FixedUpdate()
    {
		var light = this.GetComponent<Light>();
		if (!HitMaxIntensity)
		{
			if (light.intensity > MaxIntensity)
			{
				HitMaxIntensity = true;
			}
			else
			light.intensity += additiveIntensity;
			
		}
		else
		{
			if (light.intensity < MinIntensty)
			{
				HitMaxIntensity = false;
			}
			else
				light.intensity -= additiveIntensity;
			
		}
	}
}
