using UnityEngine;
using System.Collections;

public class Targeting : MonoBehaviour
{

	void Update()
	{
		if(target != null && target.gameObject.activeInHierarchy == false)
			target = null;

		if(autoTarget && target == null)
		{
			GameObject targetObject = GameObject.Find(autoTargetPath);

			if(targetObject != null)
			{
				target = targetObject.transform;
			}
		}
	}

	public bool autoTarget = false;
	public string autoTargetPath = "";

	public Transform target { get; set; }

}