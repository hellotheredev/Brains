using UnityEngine;
using System.Collections;

using Sequences;
using HelloThere.InCommon;
using HelloThere.ProgrammableValidation;
using Brains;

namespace Brains.States
{

	public class TriggerSequencePlayer : BrainState
	{
		
		override protected void OnValidateState(OperationResultHandler resultHandler)
		{
			AssertNotNull(sequencePlayer, "Sequence Player");
		}

		override public void OnNodeBegin()
		{
			if(!sequencePlayer.isPlaying)
				sequencePlayer.Play();
		}

		override public void OnNodeEnd()
		{
		}

		public SequencePlayer sequencePlayer;

	}

}