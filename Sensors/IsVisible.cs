using UnityEngine;

using HelloThere.InCommon;
using HelloThere.ProgrammableValidation;

using Brains;
using Brains.Core;

namespace Brains.Sensors
{

	public class IsVisible : Sensor
	{

		override protected void OnValidateSensor(OperationResultHandler resultHandler)
		{
			AssertNotNull(renderer, "Renderer");
		}

		override protected void ResetParameter(Parameter parameter)
		{
			parameter.boolValue = false;
		}

		override protected void UpdateParameter(Parameter parameter)
		{
			parameter.boolValue = renderer.isVisible;
		}

		override protected ParameterType parameterType
		{
			get
			{
				return ParameterType.Bool;
			}
		}

		new public Renderer renderer;

	}

}