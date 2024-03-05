using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTrigger : MonoBehaviour
{
	//Left gate or right gate?
	[SerializeField]
	int gateID = 0;

	[SerializeField]
	GateController gateController;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			gateController.GateTriggered(gateID);
		}
	}
}
