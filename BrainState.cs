using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Brains
{

	public abstract class BrainState : MonoBehaviour
	{
		
		public abstract void OnNodeBegin();
		public abstract void OnNodeEnd();

		public BrainJar knodMachine;
		public ActivationSettings activationSettings;

		[System.Serializable]
		public class ActivationSettings
		{
			public List<string> nodes;
			public bool inverted = false;
		}

	}

}