using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class BreadcrumTrail : MonoBehaviour
{
    public int MaxBreadcrums = 5;
    public float BreadcrumSpacing = 1.0f;

    private Queue<Vector3> Breadcrums = new Queue<Vector3>();
    Vector3? LastBreadcrum;
    [HideInInspector]
    public float BreadcrumSpacingSqr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BreadcrumSpacingSqr = BreadcrumSpacing * BreadcrumSpacing;
        DropBreadcrum();
    }

    // Update is called once per frame
    void Update()
    {
        if (Breadcrums.Count > 0)
        {
            if (!LastBreadcrum.HasValue)
            {
                LastBreadcrum = transform.position;
            }
            float DistanceSqr = (LastBreadcrum.Value - transform.position).sqrMagnitude;
            if (DistanceSqr >= BreadcrumSpacingSqr)
            {
                if (Breadcrums.Count < MaxBreadcrums)
                {
                    DropBreadcrum();
                }
                else
                {
                    Debug.LogWarning("Exceeded the maximum number of breadcrums (" + MaxBreadcrums + ") for member " + transform.name);
                }
            }
        }
        else
        {
            Debug.LogWarning("Ran out of breadcrums for member " + transform.name + ". Dropping new breadcrum.");
            DropBreadcrum();
        }
    }

    void DropBreadcrum()
    {
        Breadcrums.Enqueue(transform.position);
        LastBreadcrum = transform.position;
    }

    public Vector3 ConsumeBreadcrum()
    {
        if (Breadcrums.Count > 0)
        {
            return Breadcrums.Dequeue();
        }

        Debug.LogWarning("Ran out of breadcrums for member " + transform.name + ". Returning current position.");
        return transform.position;
    }
}
