using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
    public float FOV = 23;
    bool forceSway = false;

    public CameraState state;

    public enum CameraState { IDLE, SWAY, MOVING, SHAKE, SWEEP, MAP }


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
        if (BattleSystem.Instance != null)
        {
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
        }
        else if (RestSite.Instance != null)
        {
            PositonToMoveTo = RestSite.Instance.camPos;
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
                if (BattleSystem.Instance != null)
                {
                    if (!forceSway)
                    {
                        if (BattleSystem.Instance.state == BattleStates.BATTLE || BattleSystem.Instance.state == BattleStates.WON || BattleSystem.Instance.state == BattleStates.TALKING)
                            state = CameraState.IDLE;
                        else
                            state = CameraState.SWAY;
                    }
                    else
                    {
                        state = CameraState.SWAY;
                        forceSway = false;
                    }
                        
                }
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
        else if (state == CameraState.SWAY)
        {
            smoothingTime += Time.deltaTime;
            if (!DoneMovingUp)
            {
                camTransform.transform.position = Vector3.Lerp(camTransform.transform.position, new Vector3(camTransform.transform.position.x, originalPos.y + amountToSway, originalPos.z), smoothingTime / TimeDivider);
                if (camTransform.position.y >= originalPos.y + amountToSway - .1f)
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
        if (state == CameraState.MAP)
        {
            float step = 10 * Time.deltaTime;
            var boundPos = new Vector3(
                    Mathf.Clamp(followTarget.transform.position.x + followDisplacement.x, MapController.Instance.MinMapBounds.x, MapController.Instance.MaxMapBounds.x),
                    Mathf.Clamp(followTarget.transform.position.y + followDisplacement.y, MapController.Instance.MinMapBounds.y, MapController.Instance.MaxMapBounds.y),
                    Mathf.Clamp(followTarget.transform.position.z + followDisplacement.z, MapController.Instance.MinMapBounds.z, MapController.Instance.MaxMapBounds.z));
            //Zoom In
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && FOV > MapController.Instance.MinZoom && !Tools.CheckUiBlockers())
			{
                FOV -= MapController.Instance.ZoomAmount;
            }
            //Zoom Out
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && FOV < MapController.Instance.MaxZoom && !Tools.CheckUiBlockers())
            {
                FOV += MapController.Instance.ZoomAmount;
            }

            if (Input.GetMouseButton(1))
            {
                followDisplacement = new Vector3(
                Mathf.Clamp(followDisplacement.x - Input.GetAxis("Mouse X") * OptionsManager.Instance.mapSensitivityMultiplier, MapController.Instance.MinMapBounds.x - this.transform.position.x, MapController.Instance.MaxMapBounds.x + this.transform.position.x),
                   Mathf.Clamp(followDisplacement.y - Input.GetAxis("Mouse Y")  * OptionsManager.Instance.mapSensitivityMultiplier, -3, 32),
                   Mathf.Clamp(followDisplacement.z -Input.GetAxis("Mouse Y")  * OptionsManager.Instance.mapSensitivityMultiplier, -89f, -48f));

            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                followDisplacement = new Vector3(0, MapController.Instance.MinZoom, -MapController.Instance.MinZoom * 3.4f);
            }
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, FOV, step);
            transform.position = Vector3.LerpUnclamped(transform.position, boundPos, step);
        }

    }


    public void MoveToUnit(Unit unit, float xOffset = 0, float yOffset = 0, float zOffset = 0, bool useDefaultOffset = true, float MovingTimeDivider = 1f)
    {
        state = CameraState.MOVING;
        smoothingTime = 0f;
        this.MovingTimeDivider = MovingTimeDivider;
        var sprite = unit.GetComponent<SpriteRenderer>();
        if (useDefaultOffset)
        {
            PositonToMoveTo.x = sprite.bounds.center.x / 5f;
            PositonToMoveTo.y = camTransform.position.y;
            PositonToMoveTo.z = camTransform.position.z;
        }
        else
        {
            if (unit.IsPlayerControlled)
                xOffset *= -1;

            PositonToMoveTo.x = sprite.bounds.center.x + xOffset;
            PositonToMoveTo.y = sprite.bounds.center.y + yOffset;
            PositonToMoveTo.z = unit.transform.position.z + zOffset;
            print("POSITION Z: " + unit.transform.position.z);
            print("POSITION Y: " + sprite.bounds.center.y);
            print("Y OFFSET: " + yOffset);
            print("Z OFFSET: " + zOffset);
            print(PositonToMoveTo);
        }
      
    }

    public void MoveAndFollowGameObject(GameObject gameObject, Vector3 followDisplacement)
    {
        this.followTarget = gameObject;
        this.followDisplacement = followDisplacement;
        state = CameraState.MAP;
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

    public void ResetPosition(bool forceSWAY = false)
    {
        smoothingTime = 0f;
        state = CameraState.MOVING;
        PositonToMoveTo = originalPos;
        PositonToMoveTo.y = originalPos.y;
        PositonToMoveTo.z = originalPos.z;
        if(forceSWAY)
        {
            forceSway = true;
        }
    }
    public void Shake(float newShakeDuration, float newShakeAmount)
    {
        state = CameraState.SHAKE;
        shakeDuration = newShakeDuration;
        shakeAmount = newShakeAmount;
        originalLocalPos = camTransform.localPosition;

    }
}
