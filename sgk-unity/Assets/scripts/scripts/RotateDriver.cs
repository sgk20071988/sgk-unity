using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]
	public class RotateDriver : MonoBehaviour
	{
		[Tooltip( "If true, the drive will stay manipulating as long as the button is held down, if false, it will stop if the controller moves out of the collider" )]
		public bool hoverLock = false;

		[Tooltip( "If true, the transform of the GameObject this component is on will be rotated accordingly" )]
		public bool rotateGameObject = true;

		[Tooltip( "The output angle value of the drive in degrees, unlimited will increase or decrease without bound, take the 360 modulus to find number of rotations" )]
		public float outAngle;

		private Quaternion start;

		private Vector3 worldPlaneNormal = new Vector3( 1.0f, 0.0f, 0.0f );
		private Vector3 localPlaneNormal = new Vector3( 1.0f, 0.0f, 0.0f );

		private Vector3 lastHandProjected;

		private bool driving = false;

		private Hand handHoverLocked = null;
        private Interactable interactable;

		//-------------------------------------------------
	    private void Awake()
        {
            interactable = this.GetComponent<Interactable>();
        }

        //-------------------------------------------------
        private void Start()
		{
			worldPlaneNormal = new Vector3( 1.0f, 1.0f, 0.0f );

			localPlaneNormal = worldPlaneNormal;

			if ( transform.parent )
			{
				worldPlaneNormal = transform.parent.localToWorldMatrix.MultiplyVector( worldPlaneNormal ).normalized;
			}
		
			//start = Quaternion.AngleAxis( transform.localEulerAngles[0], localPlaneNormal );
			start = Quaternion.AngleAxis( transform.localEulerAngles[0], worldPlaneNormal );
			//start = Quaternion.identity;
			outAngle = 0.0f;

			UpdateAll();
		}

		//-------------------------------------------------
		void OnDisable()
		{
			if ( handHoverLocked )
			{
                handHoverLocked.HideGrabHint();
				handHoverLocked.HoverUnlock(interactable);
				handHoverLocked = null;
			}
		}

		//-------------------------------------------------
		private void OnHandHoverBegin( Hand hand )
		{
            hand.ShowGrabHint();
		}

		//-------------------------------------------------
		private void OnHandHoverEnd( Hand hand )
		{
            hand.HideGrabHint();

			driving = false;
			handHoverLocked = null;
		}

        private GrabTypes grabbedWithType;
		//-------------------------------------------------
		private void HandHoverUpdate( Hand hand )
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();
            bool isGrabEnding = hand.IsGrabbingWithType(grabbedWithType) == false;

            if (grabbedWithType == GrabTypes.None && startingGrabType != GrabTypes.None)
            {
                grabbedWithType = startingGrabType;
                // Trigger was just pressed
                lastHandProjected = ComputeToTransformProjected( hand.hoverSphereTransform );

				if ( hoverLock )
				{
					hand.HoverLock(interactable);
					handHoverLocked = hand;
				}

				driving = true;

				ComputeAngle( hand );
				UpdateAll();

                hand.HideGrabHint();
			}
            else if (grabbedWithType != GrabTypes.None && isGrabEnding)
			{
				// Trigger was just released
				if ( hoverLock )
				{
					hand.HoverUnlock(interactable);
					handHoverLocked = null;
				}

                driving = false;
                grabbedWithType = GrabTypes.None;
            }

            if ( driving && isGrabEnding == false && hand.hoveringInteractable == this.interactable )
			{
				ComputeAngle( hand );
				UpdateAll();
			}
		}


		//-------------------------------------------------
		private Vector3 ComputeToTransformProjected( Transform xForm )
		{
			Vector3 toTransform = ( xForm.position - transform.position ).normalized;
			Vector3 toTransformProjected = new Vector3( 0.0f, 0.0f, 0.0f );

			// Need a non-zero distance from the hand to the center of the CircularDrive
			if ( toTransform.sqrMagnitude > 0.0f )
			{
				toTransformProjected = Vector3.ProjectOnPlane( toTransform, worldPlaneNormal ).normalized;
			}
			else
			{
				Debug.LogFormat("<b>[SteamVR Interaction]</b> The collider needs to be a minimum distance away from the CircularDrive GameObject {0}", gameObject.ToString() );
				Debug.Assert( false, string.Format("<b>[SteamVR Interaction]</b> The collider needs to be a minimum distance away from the CircularDrive GameObject {0}", gameObject.ToString() ) );
			}

			return toTransformProjected;
		}

		private void UpdateGameObject()
		{
			if ( rotateGameObject )
			{
				transform.localRotation = start * Quaternion.AngleAxis( outAngle, localPlaneNormal );
			}
		}

		private void UpdateAll()
		{
			UpdateGameObject();
		}


		//-------------------------------------------------
		// Computes the angle to rotate the game object based on the change in the transform
		//-------------------------------------------------
		private void ComputeAngle( Hand hand )
		{
			Vector3 toHandProjected = ComputeToTransformProjected( hand.hoverSphereTransform );

			if ( !toHandProjected.Equals( lastHandProjected ) )
			{
				float absAngleDelta = Vector3.Angle( lastHandProjected, toHandProjected );

				if ( absAngleDelta > 0.0f )
				{
					Vector3 cross = Vector3.Cross( lastHandProjected, toHandProjected ).normalized;
					float dot = Vector3.Dot( worldPlaneNormal, cross );

					float signedAngleDelta = absAngleDelta;

					if ( dot < 0.0f )
					{
						signedAngleDelta = -signedAngleDelta;
					}

					outAngle += signedAngleDelta;
					lastHandProjected = toHandProjected;	
				}
			}
		}
	}
}
