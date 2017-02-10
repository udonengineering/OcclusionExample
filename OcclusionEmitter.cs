using UnityEngine;

[RequireComponent(typeof(AudioLowPassFilter))]
[RequireComponent(typeof(AudioSource))]
public class OcclusionEmitter : MonoBehaviour
{
    [SerializeField]
    private LayerMask occlusionMask;
    [SerializeField]
    private Vector3[] occlusionPositions;
    [SerializeField]
    private AnimationCurve occlusionRamp;
    [SerializeField]
    private bool debug = false;

    private AudioSource source;
    private AudioLowPassFilter lowPass;
    private float occlusionFrequency = 22000f;
    private static Transform UserTrans;
    private const float OcclusionUpdateSpeed = 2f;

    private void Start()
    {
        if (UserTrans == null)
        {
            SetUserToCamera();
        }

        this.source = GetComponent<AudioSource>();
        this.lowPass = GetComponent<AudioLowPassFilter>();
    }

    private void Update()
    {
        UpdateOcclusion();
        SmoothOcclusionFrequency();
    }

    public void SetUserToCamera()
    {
        UserTrans = Camera.main.transform;
    }

    private void UpdateOcclusion()
    {
        this.transform.LookAt(UserTrans);

        int amountOccluded = 0;

        for (int i = 0; i < this.occlusionPositions.Length; i++)
        {
            Vector3 startPos = this.transform.position + this.transform.TransformDirection(this.occlusionPositions[i]);
            if (Physics.Linecast(startPos, UserTrans.position, this.occlusionMask))
            {
                amountOccluded++;
            }

            if (this.debug)
            {
                Debug.DrawLine(startPos, UserTrans.position);
            }
        }

        float occlusionPercentage = amountOccluded / this.occlusionPositions.Length;
        this.occlusionFrequency = this.occlusionRamp.Evaluate(occlusionPercentage);
    }

    private void SmoothOcclusionFrequency()
    {
        this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, this.occlusionFrequency, Time.deltaTime * OcclusionUpdateSpeed);
    }
}