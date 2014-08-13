using UnityEngine;

using HelloThere.InCommon;
using HelloThere.ProgrammableValidation;
using Brains.Core;

namespace Brains
{

	public abstract class Sensor : PVMonoBehaviour
	{

		override protected void OnValidation(OperationResultHandler resultHandler)
		{
			if(AssertNotNull(brainJar, "Brain Jar"))
			{
				if(AssertNotNull(brainJar.brain, "Brain"))
				{
					if(AssertTrue(brainJar.brain.HasParameter(brainParameter), "Missing Brain Parameter: " + brainParameter))
					{
						AssertTrue(brainJar.brain.GetParameter(brainParameter).type == parameterType, "The " + brainParameter + " parameter must be of type " + parameterType.ToString());
					}
				}
			}


			OnValidateSensor(resultHandler);
		}

		protected abstract void OnValidateSensor(OperationResultHandler resultHandler);
		protected abstract void ResetParameter(Parameter parameter);
		protected abstract void UpdateParameter(Parameter parameter);
		protected abstract ParameterType parameterType { get; }

		void Update()
		{
			if(brainParameterHash == 0)
				brainParameterHash = brainJar.brain.GetParameterHashCode(brainParameter);

			UpdateParameter(brainJar.brain.GetParameter(brainParameterHash));
		}

		public void Reset()
		{
			if(brainParameterHash == 0)
				brainParameterHash = brainJar.brain.GetParameterHashCode(brainParameter);

			ResetParameter(brainJar.brain.GetParameter(brainParameterHash));
		}

		public BrainJar brainJar;
		public string brainParameter;

		private int brainParameterHash;

	}

}