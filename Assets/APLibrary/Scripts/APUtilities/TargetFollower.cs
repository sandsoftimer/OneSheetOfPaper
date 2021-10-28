using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollower : APBehaviour
{
    public Transform followTarget, lookTarget;

    public UpdateMethod updateMethod;
    [Header("Follow Properties")]
    public bool follow;
    [Range(0f, 1f)]
    public float followSmoothness;
    public Vector3 followOffset;
    public AcceptedValueType followX, followY, followZ;

    [Header("Look Properties")]
    public bool lookAtTarget;
    [Range(0f, 1f)]
    public float lookSmoothness;
    public Vector3 lookOffset;
    public AcceptedValueType lookX, lookY, lookZ;

    Vector3 velocity;

    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public virtual void Update()
    {
        if (!updateMethod.Equals(UpdateMethod.UPDATE))
            return;

        UpdateTransform();
    }

    public virtual void FixedUpdate()
    {
        if (!updateMethod.Equals(UpdateMethod.FIXED_UPDATE))
            return;

        UpdateTransform();
    }

    #endregion ALL UNITY FUNCTIONS
    //=================================   
    #region ALL OVERRIDING FUNCTIONS


    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS

    void UpdateTransform()
    {
        if (follow)
        {
            Vector3 nextPosition = Vector3.SmoothDamp(transform.position, followTarget.position + followOffset, ref velocity, followSmoothness);
            ValidateAxisValue(ref nextPosition.x, transform.position.x, followX);
            ValidateAxisValue(ref nextPosition.y, transform.position.y, followY);
            ValidateAxisValue(ref nextPosition.z, transform.position.z, followZ);

            transform.position = nextPosition;
        }

        if (lookAtTarget)
        {
            var targetRotation = Quaternion.LookRotation(lookTarget.transform.position + lookOffset - transform.position);
            //ValidateAxisValue(ref targetRotation.x, transform.rotation.x, lookX);
            //ValidateAxisValue(ref targetRotation.y, transform.rotation.y, lookY);
            //ValidateAxisValue(ref targetRotation.z, transform.rotation.z, lookZ);

            transform.rotation = Quaternion.Slerp(targetRotation, transform.rotation, lookSmoothness);
        }
    }

    void ValidateAxisValue(ref float t, float currentValue, AcceptedValueType acceptedValueType)
    {
        switch (acceptedValueType)
        {
            case AcceptedValueType.BOTH:
                break;
            case AcceptedValueType.ONLY_POSITIVE:
                if (t < currentValue) t = currentValue;
                break;
            case AcceptedValueType.ONLY_NEGETIVE:
                if (t > currentValue) t = currentValue;
                break;
            case AcceptedValueType.NONE:
                t = currentValue;
                break;
        }
    }

    #endregion ALL SELF DECLEAR FUNCTIONS

}
