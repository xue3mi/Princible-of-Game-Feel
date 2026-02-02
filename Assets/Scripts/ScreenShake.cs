using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private float shakeTimer;
    private float shakeMagnitude;
    private float shakeIntervalTimer;
    //how long single shake is
    [SerializeField] private float shakeIntervalTimerMax = 0.05f;
    private bool shakeDirectionForward = true;

    [SerializeField] private float defaultShakeDuration = 0.3f;
    [SerializeField] private ShakeType shakeType;
    private Vector3 shakeDirection;
    private bool punching;
    private float pullBackSpeed = 10f;

    public enum ShakeType
    {
        Unchanged, Horizontal, Vertical, Diagonal, Random, PunchUp, PunchDown, PunchLeft, PunchRight
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) BeginShake();
        if (Input.GetKeyDown(KeyCode.K)) BeginShake(1f,0.01f,ShakeType.Diagonal);
        if (Input.GetKeyDown(KeyCode.P)) BeginPunch();
        if (Input.GetKeyDown(KeyCode.O)) BeginPunch(3, 20f, ShakeType.PunchRight);

        if (punching)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, pullBackSpeed * Time.deltaTime);
            //if reached back
            if (transform.localPosition == Vector3.zero) punching = false;
        }

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeType == ShakeType.Random)
            {
                transform.localPosition = (Vector3)UnityEngine.Random.insideUnitCircle * shakeMagnitude;
            }
            else //horizontal vertical or diagonal
            {
                // ask T/F"?"; if T give 1, if F give -1; then multiply by magnitude
                transform.localPosition = shakeDirection * (shakeDirectionForward ? 1 : -1) * shakeMagnitude;
                shakeIntervalTimer -= Time.deltaTime;
                if (shakeIntervalTimer <= 0)
                {
                    //flipping to opposite direction so coming back
                    shakeDirectionForward = !shakeDirectionForward;
                    shakeIntervalTimer = shakeIntervalTimerMax;
                }
            }
            if (shakeTimer > 0) transform.localPosition = Vector3.zero;
        }
    }
    public void BeginShake(float duration = 0.2f, float magnitude = 0.05f, ShakeType type = ShakeType.Unchanged)
    {
        transform.localPosition = Vector3.zero;

        shakeTimer = defaultShakeDuration;
        shakeMagnitude = magnitude;
        shakeType = type;
        if(type != ShakeType.Unchanged) shakeType = type;
        shakeIntervalTimer = shakeIntervalTimerMax / 2;

        if (shakeType == ShakeType.Horizontal) shakeDirection = Vector3.right;
        else if (shakeType == ShakeType.Vertical) shakeDirection = Vector3.up;
        else if (shakeType == ShakeType.Diagonal) shakeDirection = new Vector3(.7f, .7f, 0);
    }

    public void BeginPunch(float punchDistance = 1.5f, float myPullBackSpeed = 10f, ShakeType type = ShakeType.Unchanged)
    {
        pullBackSpeed = myPullBackSpeed;
        punching = true;
        Vector3 punchDirection = Vector3.up;
        if (type == ShakeType.PunchUp) punchDirection = Vector3.up;
        else if (type == ShakeType.PunchDown) punchDirection = Vector3.down;
        else if (type == ShakeType.PunchLeft) punchDirection = Vector3.left;
        else if (type == ShakeType.PunchRight) punchDirection = Vector3.right;
        //hit logic
        transform.localPosition = punchDirection * punchDistance;
    }
}
