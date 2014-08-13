using UnityEngine;

using HelloThere.InCommon;
using HelloThere.ProgrammableValidation;

using Brains;
using Brains.Core;

namespace Brains.Sensors
{

	public class DistanceToTarget : Sensor
	{

		override protected void OnValidateSensor(OperationResultHandler resultHandler)
		{
			AssertNotNull(targeting, "Targeting");
			AssertNotNull(distanceFrom, "Distance From");
		}

		override protected void ResetParameter(Parameter parameter)
		{
			parameter.floatValue = Mathf.Infinity;
		}

		override protected void UpdateParameter(Parameter parameter)
		{
			Transform targetNode = null;

			if(targeting.target != null)
			{
				if(useTargetingSubNode)
				{
					Transform t = targeting.target.Find(subNodePath);

					if(t != null)
					{
						targetNode = t;
					}
					else
					{
						targetNode = targeting.target;
					}
				}
				else
				{
					targetNode = targeting.target;
				}

				parameter.floatValue = Vector3.Distance(targetNode.position, distanceFrom.position);
			}
			else
			{
				parameter.floatValue = Mathf.Infinity;
			}
		}

		override protected ParameterType parameterType
		{
			get
			{
				return ParameterType.Float;
			}
		}

		public Targeting targeting;
		public bool useTargetingSubNode = false;
		public string subNodePath = "";
		public Transform distanceFrom;

	}

}