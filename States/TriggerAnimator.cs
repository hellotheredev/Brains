using UnityEngine;
using System.Collections;

using HelloThere.InCommon;
using HelloThere.ProgrammableValidation;
using Brains;

namespace Brains.States
{

	public class TriggerAnimator : BrainState
	{
		
		override protected void OnValidateState(OperationResultHandler resultHandler)
		{
			AssertNotNull(animator, "Animator");
		}

		override public void OnNodeBegin()
		{
			animator.SetTrigger(parameterName);
		}

		override public void OnNodeEnd()
		{
		}

		public Animator animator;
		public string parameterName;

	}

}