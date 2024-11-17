using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RotorTipTrailController : MonoBehaviour
{
    public List<TrailRenderer> tipTrails;
    public Gradient defaultGradient;
    public Gradient sprintGradient;

    public bool IsEmitting { get; private set; }

    private void Start()
    {
        for (int i = 0; i < tipTrails.Count; i++)
        {
            tipTrails[i].colorGradient = defaultGradient;
        }
    }

    public void SetIsEmitting(bool value)
    {
        IsEmitting = value;
        for (int i = 0; i < tipTrails.Count; i++)
        {
            if (value)
                tipTrails[i].colorGradient = sprintGradient;
            else
                tipTrails[i].colorGradient = defaultGradient;
        }
    }

    public void SetIsActive(bool value)
    {
        for (int i = 0; i < tipTrails.Count; i++)
        {
            tipTrails[i].emitting = value;
        }
    }
}
