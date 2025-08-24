using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Vector3 Amount = new Vector3(1f, 1f, 0f);
    public float Duration = 1f;
    public float Speed = 10f;
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    public bool DeltaMovement = true;

    private Camera cam;
    private float timer = 0f;
    private Vector3 prevPos, currPos;
    private float prevFoV, currFoV;
    private bool autoDestroy;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public static void ShakeOnce(float duration = 1f, float speed = 10f, Vector3? amount = null, Camera camera = null, bool deltaMovement = true, AnimationCurve curve = null)
    {
        CameraShake shake = ((camera != null) ? camera : Camera.main).gameObject.AddComponent<CameraShake>();
        shake.Duration = duration;
        shake.Speed = speed;
        if (amount.HasValue) shake.Amount = amount.Value;
        if (curve != null) shake.Curve = curve;
        shake.DeltaMovement = deltaMovement;
        shake.autoDestroy = true;
        shake.Begin();
    }

    public void Begin()
    {
        ResetCamera();
        timer = Duration;
    }

    private void LateUpdate()
    {
        if (timer <= 0f) return;

        timer -= Time.deltaTime;
        if (timer > 0f)
        {
            float t = 1f - (timer / Duration);
            float curveVal = Curve.Evaluate(t);

            currPos =
                (Mathf.PerlinNoise(timer * Speed, timer * Speed * 2f) - 0.5f) * Amount.x * transform.right * curveVal +
                (Mathf.PerlinNoise(timer * Speed * 2f, timer * Speed) - 0.5f) * Amount.y * transform.up * curveVal;

            currFoV =
                (Mathf.PerlinNoise(timer * Speed * 2f, timer * Speed * 2f) - 0.5f) * Amount.z * curveVal;

            cam.fieldOfView += (currFoV - prevFoV);
            cam.transform.Translate(DeltaMovement ? (currPos - prevPos) : currPos);

            prevPos = currPos;
            prevFoV = currFoV;
        }
        else
        {
            ResetCamera();
            if (autoDestroy) Destroy(this);
        }
    }

    private void ResetCamera()
    {
        cam.transform.Translate(DeltaMovement ? -prevPos : Vector3.zero);
        cam.fieldOfView -= prevFoV;

        prevPos = currPos = Vector3.zero;
        prevFoV = currFoV = 0f;
    }
}