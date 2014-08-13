using UnityEngine;

using HelloThere.InCommon;
using HelloThere.ProgrammableValidation;
using Brains;

namespace Brains.States
{

	public class TargetApproach : BrainState
	{
		
		override protected void OnValidateState(OperationResultHandler resultHandler)
		{
			if(!AssertNotNull(targeting, "Targeting"))
				return;

			if(!AssertNotNull(rigidbody, "Rigidbody"))
				return;
		}
		
		override public void OnNodeBegin()
		{
			targetNode = null;
		}

		void FixedUpdate()
		{	
			if(targeting == null || targeting.target == null) return;

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

			if(targetNode != null && rigidbody != null)
			{
				Vector3 error = targetNode.position - transform.position;
				Vector3 direction = error.normalized;

				if(flee)
					direction *= -1;

				Debug.DrawRay(transform.position, direction);

				if(onlyWhenFacingTarget)
				{
					// todo: more control over approach angle
					Vector3 wantedAim = Vector3.Normalize(targetNode.position - transform.position);
					float headingDot = Mathf.Pow(Mathf.Clamp01(Vector3.Dot(transform.forward, wantedAim)), 2f);

					rigidbody.AddForce(direction * force * headingDot, forceMode);
				}
				else
				{
					rigidbody.AddForce(direction * force, forceMode);
				}
			}
		}

		override public void OnNodeEnd()
		{

		}

		private Transform targetNode = null;

		public Targeting targeting;
		public bool useTargetingSubNode = false;
		public string subNodePath = "";

		new public Rigidbody rigidbody;
		public ForceMode forceMode;
		public float force = 1;
		public bool flee = false;

		public bool onlyWhenFacingTarget = false;

	}

}