using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Cinemachine;

public class LabCamera : MonoBehaviour
{
	public static LabCamera Instance { get; private set; }

	public Transform camTransform;
	public Camera cam;
	public Camera uicam;

	public float shakeDuration = 0f;
	[SerializeField]
	private bool DoneMovingUp = false;
	[SerializeField]
	private float TimeDivider = 100f;
    public float MovingTimeDivider = 10f;
    [SerializeField]
	private float amountToSway = 0.5f;
	public float shakeAmount = 100f;
	public float decreaseFactor = 0.1f;
	public Vector3 originalLocalPos;
	public float smoothingTime = 0.5f;
	Vector3 originalPos;
	private Vector3 PositonToMoveTo;
	public GameObject followTarget;
	public Vector3 followDisplacement;

	public CameraState state;

	public enum CameraState { IDLE, SWAY, MOVING, SHAKE, SWEEP, FOLLOW}


	void Awake()
	{
		
		if (Instance != null)
			{
				Destroy(this);
			}
			else
			{
				camTransform = GetComponent<Camera>().transform;
				Instance = this;
			    state = CameraState.IDLE;
			}

	}
	

    public void ReadjustCam()
	{
        smoothingTime = 0f;
        state = CameraState.MOVING;
        if (BattleSystem.Instance.playerPositions[2].GetComponent<BattleSpawnPoint>().unit != null || BattleSystem.Instance.enemyPositions[2].GetComponent<BattleSpawnPoint>().unit != null)
        {
            PositonToMoveTo = BattleSystem.Instance.cameraPos3Units;
            print("Camera is sized for 3 units");

        }
        else if (BattleSystem.Instance.playerPositions[1].GetComponent<BattleSpawnPoint>().unit != null || BattleSystem.Instance.enemyPositions[1].GetComponent<BattleSpawnPoint>().unit != null)
        {
            PositonToMoveTo = BattleSystem.Instance.cameraPos2Units;
            uicam.transform.position = new Vector3(uicam.transform.position.x, uicam.transform.position.y, uicam.transform.position.z);
            print("Camera is sized for 2 units");

        }
        else
        {
            print("Camera is sized for 1 unit");
			PositonToMoveTo = BattleSystem.Instance.cameraPos1Units;
            uicam.transform.position = new Vector3(uicam.transform.position.x, uicam.transform.position.y, uicam.transform.position.z);


        }
        originalPos = PositonToMoveTo;
		
    }

    private void Update()
    {
		if (Input.GetMouseButtonUp(1) && camTransform.position.x != originalPos.x && BattleSystem.Instance != null)
		{
			ResetPosition();
		}
	}

    private void LateUpdate()
    {
		if (state == CameraState.MOVING)
		{
			if (smoothingTime > 1)
			{
				if(BattleSystem.Instance != null)
				state = CameraState.SWAY;
				else
                    state = CameraState.IDLE;

                smoothingTime = 0f;
			}
			else
			{
				smoothingTime += Time.deltaTime / MovingTimeDivider;
				camTransform.transform.position = Vector3.Lerp(camTransform.transform.position, PositonToMoveTo, smoothingTime);
			}
		}
		else if (state == CameraState.SWAY && BattleSystem.Instance.state != BattleStates.BATTLE)
		{
            smoothingTime += Time.deltaTime;
			if (!DoneMovingUp)
			{
				camTransform.transform.position = Vector3.Lerp(camTransform.transform.position, new Vector3(camTransform.transform.position.x, originalPos.y + amountToSway, originalPos.z), smoothingTime / TimeDivider);
				if(camTransform.position.y >= originalPos.y + amountToSway - .1f)
                {
					DoneMovingUp = true;
					smoothingTime = 0f;
                }
			}
			else
			{
				camTransform.transform.position = Vector3.Lerp(camTransform.transform.position, new Vector3(camTransform.transform.position.x, originalPos.y - amountToSway * 2, originalPos.z), smoothingTime / TimeDivider);
				if (camTransform.position.y <= originalPos.y - amountToSway * 2 + .1f)
				{
					DoneMovingUp = false;
					smoothingTime = 0f;
				}
			}
			
			
			
		}
		if (state == CameraState.SHAKE)
		{
			if (shakeDuration > 0)
			{
				camTransform.localPosition = originalLocalPos + Random.insideUnitSphere * shakeAmount;

				shakeDuration -= Time.deltaTime * decreaseFactor;
			}
			else
			{
				shakeDuration = 1f;
				camTransform.localPosition = originalLocalPos;
				state = CameraState.IDLE;
			}
		}
        if (state == CameraState.FOLLOW)
        {
            float step = 10 * Time.deltaTime;
            transform.position = Vector3.LerpUnclamped(transform.position, new Vector3(followTarget.transform.position.x, followTarget.transform.position.y + followDisplacement.y, followTarget.transform.position.z + followDisplacement.z), step);
        }

    }

	
	public void MoveToUnit(Unit unit, float xOffset = 0, float yOffset = 0, float zOffset = 0)
    {
        state = CameraState.MOVING;
		smoothingTime = 0f;
		PositonToMoveTo = unit.transform.position / 5f;
		if(xOffset != 0)
		{
			if (unit.IsPlayerControlled)
				xOffset *= -1;
            PositonToMoveTo.x = camTransform.position.x + xOffset;
        }
        PositonToMoveTo.y = camTransform.position.y + yOffset;
        PositonToMoveTo.z = camTransform.position.z + zOffset;
    }

    public void MoveAndFollowGameObject(GameObject gameObject)
    {
		this.followTarget = gameObject;
    }

    public void MoveToGameObject(GameObject gameObject)
    {
        state = CameraState.MOVING;
        smoothingTime = 0f;
        PositonToMoveTo.x = gameObject.transform.position.x;
        PositonToMoveTo.y = camTransform.position.y;
        PositonToMoveTo.z = camTransform.position.z;
    }

    public IEnumerator DoSlowHorizontalSweep()
    {
		LabCamera.Instance.state = CameraState.SWEEP;
        while (LabCamera.Instance.state == LabCamera.CameraState.SWEEP)
        {
            LabCamera.Instance.transform.position += new Vector3(0.01f, 0, 0);
            yield return new WaitForSeconds(0.0001f);
        }
    }

    public void ResetPosition()
	{
        smoothingTime = 0f;
		state = CameraState.MOVING;
		PositonToMoveTo = originalPos;
		PositonToMoveTo.y = originalPos.y;
        PositonToMoveTo.z = originalPos.z;
	}
	public  void Shake(float newShakeDuration, float newShakeAmount)
	{
		state = CameraState.SHAKE;
		shakeDuration = newShakeDuration;
		shakeAmount = newShakeAmount;
		originalLocalPos = camTransform.localPosition;

    }
}
