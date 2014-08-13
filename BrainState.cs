using UnityEngine;
using System.Collections.Generic;

using HelloThere.InCommon;
using HelloThere.ProgrammableValidation;

namespace Brains
{

	public abstract class BrainState : PVMonoBehaviour
	{
		
		override protected void OnValidation(OperationResultHandler resultHandler)
		{
			if(!AssertNotNull(brainJar, "Brain Jar") || !AssertNotNull(brainJar.brain, "Brain"))
				return;

			AssertAtLeast(activationSettings.nodes, "Activation Settings", 1);

			foreach(string nodeName in activationSettings.nodes)
			{
				if(!AssertTrue(brainJar.brain.HasNode(nodeName), "Brain does not contain a node named: " + nodeName))
					return;
			}

			OnValidateState(resultHandler);
		}

		protected abstract void OnValidateState(OperationResultHandler resultHandler);

		public abstract void OnNodeBegin();
		public abstract void OnNodeEnd();

		public BrainJar brainJar;
		public ActivationSettings activationSettings;

		[System.Serializable]
		public class ActivationSettings
		{
			public List<string> nodes;
			public bool inverted = false;
		}

	}

}