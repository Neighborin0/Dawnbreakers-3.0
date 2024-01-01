using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cinemachine;
using UnityEngine.SceneManagement;

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
    public Vector3 originalPos;
    public Vector3 PositonToMoveTo;
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
            camTransform = transform;
            Instance = this;
            state = CameraState.IDLE;
        }
    }

    public void Start()
    {
        if(SceneManager.GetActiveScene().name == "MAP2")
        {
            this.GetComponent<CinemachineConfiner>().m_BoundingVolume = MapController.Instance.mapCollider;
        }
    }

    public void ReadjustCam()
    {
        smoothingTime = 0f;
        state = CameraState.MOVING;
        if (BattleSystem.Instance != null)
        {
            if (BattleSystem.Instance.BossNode)
            {
                PositonToMoveTo = BattleSystem.Instance.bossNodeCamPos;
            }
            else
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


        }
        else if (RestSite.Instance != null)
        {
            PositonToMoveTo = RestSite.Instance.camPos;
        }
        originalPos = PositonToMoveTo;

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
                    followTarget.transform.position.x + followDisplacement.x,
                    followTarget.transform.position.y + followDisplacement.y,
                    followTarget.transform.position.z + followDisplacement.z);
            //Zoom In
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && FOV > MapController.Instance.MinZoom && !Tools.CheckUiBlockers() && !BattleLog.Instance.characterdialog.gameObject.activeSelf)
            {
                FOV -= MapController.Instance.ZoomAmount;
            }
            //Zoom Out
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && FOV < MapController.Instance.MaxZoom && !Tools.CheckUiBlockers() && !BattleLog.Instance.characterdialog.gameObject.activeSelf)
            {
                FOV += MapController.Instance.ZoomAmount;
            }
            var virtualCam = this.GetComponent<CinemachineVirtualCamera>();
            if (Input.GetMouseButton(1) && !BattleLog.Instance.characterdialog.gameObject.activeSelf)
            {
                //this is hella jank but it's like 2 am bear with me
                var bounds = this.GetComponent<CinemachineConfiner>().m_BoundingVolume.bounds;
                if (camTransform.position.x - 0.1f > bounds.min.x && -Input.GetAxis("Mouse X") < 0)
                    followDisplacement = new Vector3(followDisplacement.x - Input.GetAxis("Mouse X") * OptionsManager.Instance.mapSensitivityMultiplier, followDisplacement.y, followDisplacement.z);
                else if (camTransform.position.x + 0.1f < bounds.max.x && -Input.GetAxis("Mouse X") > 0)
                    followDisplacement = new Vector3(followDisplacement.x - Input.GetAxis("Mouse X") * OptionsManager.Instance.mapSensitivityMultiplier, followDisplacement.y, followDisplacement.z);



                if (camTransform.position.y - 0.1f > bounds.min.y && -Input.GetAxis("Mouse Y") < 0)
                {
                    followDisplacement = new Vector3(followDisplacement.x, followDisplacement.y - Input.GetAxis("Mouse Y") * OptionsManager.Instance.mapSensitivityMultiplier, followDisplacement.z);
                    followDisplacement = new Vector3(followDisplacement.x, followDisplacement.y, followDisplacement.z - Input.GetAxis("Mouse Y") * OptionsManager.Instance.mapSensitivityMultiplier);
                }
                if (camTransform.position.y + 0.1f < bounds.max.y && -Input.GetAxis("Mouse Y") > 0)
                {
                    followDisplacement = new Vector3(followDisplacement.x, followDisplacement.y - Input.GetAxis("Mouse Y") * OptionsManager.Instance.mapSensitivityMultiplier, followDisplacement.z);
                    followDisplacement = new Vector3(followDisplacement.x, followDisplacement.y, followDisplacement.z - Input.GetAxis("Mouse Y") * OptionsManager.Instance.mapSensitivityMultiplier);
                }
            }
            if (Input.GetKeyDown(KeyCode.R) && !BattleLog.Instance.characterdialog.gameObject.activeSelf)
            {
                followDisplacement = new Vector3(0, MapController.Instance.MinZoom, -MapController.Instance.MinZoom * 3.4f);
            }
          
            virtualCam.m_Lens.FieldOfView = Mathf.Lerp(virtualCam.m_Lens.FieldOfView, FOV, step);
            transform.position = Vector3.LerpUnclamped(transform.position, boundPos, step);
        }

    }


    public void MoveToUnit(Unit unit, Vector3 overrideYPos, float xOffset = 0, float yOffset = 0, float zOffset = 0, float MovingTimeDivider = 1f, bool UsesDefaultOffset = false)
    {
        state = CameraState.MOVING;
        smoothingTime = 0f;
        this.MovingTimeDivider = MovingTimeDivider;
        var sprite = unit.GetComponent<SpriteRenderer>();

        if (overrideYPos.y != 0)
        {
            PositonToMoveTo.x = sprite.bounds.center.x / 5f;
            PositonToMoveTo.y  = originalPos.y + unit.camOffset.y;
            PositonToMoveTo.z = originalPos.z + unit.camOffset.z;
        }
        else if(UsesDefaultOffset)
        {
            PositonToMoveTo.x = sprite.bounds.center.x / 5f;
            if (camTransform.position.y > originalPos.y)
            PositonToMoveTo.y = camTransform.position.y + unit.camOffset.y;
            else
                PositonToMoveTo.y = originalPos.y + unit.camOffset.y;
            PositonToMoveTo.z = originalPos.z + unit.camOffset.z;
        }
        else if (xOffset == 0 && yOffset == 0 && zOffset == 0)
        {
            PositonToMoveTo.y = BattleSystem.Instance.cameraPos1Units.y + unit.camOffset.y;
            PositonToMoveTo.z = BattleSystem.Instance.cameraPos1Units.z + unit.camOffset.z;
        }
        else
        {
            if (unit.IsPlayerControlled)
                xOffset *= -1;

            PositonToMoveTo.x = sprite.bounds.center.x + xOffset + unit.camOffset.x;
            PositonToMoveTo.y = sprite.bounds.center.y + yOffset + unit.camOffset.y;
            PositonToMoveTo.z = unit.transform.position.z + zOffset + unit.camOffset.z;
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

    public void MoveToPosition(Vector3 position, float MovingTimeDivider = 1f)
    {
        smoothingTime = 0f;
        state = CameraState.MOVING;
        PositonToMoveTo = position;
        this.MovingTimeDivider = MovingTimeDivider;
    }

    public void ResetPosition(bool forceSWAY = false)
    {
        smoothingTime = 0f;
        state = CameraState.MOVING;
        PositonToMoveTo = originalPos;
        PositonToMoveTo.y = originalPos.y;
        PositonToMoveTo.z = originalPos.z;
        if (forceSWAY)
        {
            forceSway = true;
        }
    }

    public void Rotate(Vector3 rotation)
    {
        StartCoroutine(Rotating(rotation));
    }

    public void ResetRotation()
    {
        StartCoroutine(Rotating(Vector3.zero));
    }

    private IEnumerator Rotating(Vector3 rotation)
    {
        if (rotation.z < 0f)
        {
            while (this.transform.eulerAngles.z != 360 + rotation.z)
            {
                this.transform.Rotate(new Vector3(0, 0, -1));
                yield return new WaitForSeconds(0.001f);
            }
        }

        else if (rotation.z >= 0f)
        {
            while (!Mathf.Approximately(this.transform.eulerAngles.z, rotation.z))
            {
                this.transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(transform.rotation.x, transform.rotation.y, rotation.z, transform.rotation.w), 0.1f);
                yield return new WaitForSeconds(0.001f);
            }
        }

    }

    public void PanCam(float x, float y, float z)
    {

    }
   
    public void Shake(float newShakeDuration, float newShakeAmount)
    {
        state = CameraState.SHAKE;
        shakeDuration = newShakeDuration;
        shakeAmount = newShakeAmount;
        originalLocalPos = camTransform.localPosition;

    }
}
