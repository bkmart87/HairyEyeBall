using UnityEngine;
using System.Collections;

public class CamerFollow : MonoBehaviour 
{
	[SerializeField] 
	Transform playerTransform;
	[SerializeField]
		Vector3 offset;

 void LateUpdate()
	{
		transform.position = playerTransform.position + offset;

	}

}
