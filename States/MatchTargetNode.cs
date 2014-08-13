using UnityEngine;

using HelloThere.InCommon;
using HelloThere.ProgrammableValidation;
using Brains;

namespace Brains.States
{

	public class MatchTargetNode : BrainState
	{
		
		override protected void OnValidateState(OperationResultHandler resultHandler)
		{
			AssertNotNull(targeting, "Targeting");
			AssertNotNull(animator, "Animator");
			AssertNotNull(modelTransform, "Model Transform");
		}

		override public void OnNodeBegin()
		{
			targetNode = null;
			controlTransformParent = modelTransform.parent;
		}

		void LateUpdate()
		{
			if(modelTransform == null) return;

			if(targetNode == null)
			{
				if(targeting.target != null)
				{
					if(useSubNodePath)
					{
						Transform t = targeting.target.Find(subNodePath);

						if(t != null)
						{
							targetNode = t;
						}
						else
						{
							Debug.LogWarning("Sub node not found. Defaulting to targeting.target");
							targetNode = targeting.target;
						}
					}
					else
					{
						targetNode = targeting.target;
					}
				}
			}

			if(targetNode == null) return;

			float d = animator.GetFloat("Match Target");

			modelTransform.position = Vector3.Lerp(controlTransformParent.position, targetNode.position, d);
			modelTransform.rotation = Quaternion.Slerp(controlTransformParent.rotation, targetNode.rotation * Quaternion.Euler(targetRotationOffset), d);
		}

		override public void OnNodeEnd()
		{
			modelTransform.localPosition = Vector3.zero;
			modelTransform.localRotation = Quaternion.identity;
		}

		private Transform targetNode;
		private Transform controlTransformParent;

		public Targeting targeting;
		public bool useSubNodePath = false;
		public string subNodePath = "Model/Rig/Root/MainSub";
		public Animator animator;
		public string animatorParameterName;
		public Transform modelTransform;
		public Vector3 targetRotationOffset = Vector3.zero;

	}

}