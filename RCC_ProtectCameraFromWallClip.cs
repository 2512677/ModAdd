using System;
using System.Collections;
using UnityEngine;

public class RCC_ProtectCameraFromWallClip : MonoBehaviour
{
	public class RayHitComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
		}
	}

	private RCC_Camera RCCCam;

	public float clipMoveTime;

	public float returnTime;

	public float sphereCastRadius = 0.1f;

	public bool visualiseInEditor;

	public float closestDistance;

	public string dontClipTag = "Player";

	private Transform m_Cam;

	private Transform m_Pivot;

	private float m_OriginalDist;

	private float m_MoveVelocity;

	private float m_CurrentDist;

	private Ray m_Ray;

	private RaycastHit[] m_Hits;

	private RayHitComparer m_RayHitComparer;

	public bool protecting
	{
		get;
		private set;
	}

	private void Start()
	{
		RCCCam = GetComponent<RCC_Camera>();
		m_Cam = GetComponentInChildren<Camera>().transform;
		m_Pivot = RCCCam.pivot.transform;
		m_OriginalDist = m_Cam.localPosition.magnitude;
		m_CurrentDist = m_OriginalDist;
		m_RayHitComparer = new RayHitComparer();
	}

	private void LateUpdate()
	{
		if (!RCCCam)
		{
			return;
		}
		if (RCCCam.cameraMode == RCC_Camera.CameraMode.FPS || RCCCam.cameraMode == RCC_Camera.CameraMode.WHEEL || RCCCam.cameraMode == RCC_Camera.CameraMode.TOP)
		{
			m_Cam.localPosition = Vector3.zero;
			return;
		}
		float num = m_OriginalDist;
		m_Ray.origin = m_Pivot.position + m_Pivot.forward * sphereCastRadius;
		m_Ray.direction = -m_Pivot.forward;
		Collider[] array = Physics.OverlapSphere(m_Ray.origin, sphereCastRadius);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].isTrigger && (!(array[i].attachedRigidbody != null) || !array[i].attachedRigidbody.CompareTag(dontClipTag)))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			m_Ray.origin += m_Pivot.forward * sphereCastRadius;
			m_Hits = Physics.RaycastAll(m_Ray, m_OriginalDist - sphereCastRadius);
		}
		else
		{
			m_Hits = Physics.SphereCastAll(m_Ray, sphereCastRadius, m_OriginalDist + sphereCastRadius);
		}
		Array.Sort(m_Hits, m_RayHitComparer);
		float num2 = float.PositiveInfinity;
		for (int j = 0; j < m_Hits.Length; j++)
		{
			if (m_Hits[j].distance < num2 && !m_Hits[j].collider.isTrigger && (!(m_Hits[j].collider.attachedRigidbody != null) || !m_Hits[j].collider.attachedRigidbody.CompareTag(dontClipTag)))
			{
				num2 = m_Hits[j].distance;
				Vector3 vector = m_Pivot.InverseTransformPoint(m_Hits[j].point);
				num = 0f - vector.z;
				flag2 = true;
			}
		}
		if (flag2)
		{
			UnityEngine.Debug.DrawRay(m_Ray.origin, -m_Pivot.forward * (num + sphereCastRadius), Color.red);
		}
		protecting = flag2;
		m_CurrentDist = Mathf.SmoothDamp(m_CurrentDist, num, ref m_MoveVelocity, (!(m_CurrentDist > num)) ? returnTime : clipMoveTime);
		m_CurrentDist = Mathf.Clamp(m_CurrentDist, closestDistance, m_OriginalDist);
		m_Cam.localPosition = -Vector3.forward * m_CurrentDist;
	}
}
