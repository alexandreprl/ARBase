using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementIndicator : MonoBehaviour
{
    [SerializeField] private GameObject validIndicator, invalidIndicator;
    private bool _stable;

    public bool Stable
    {
        set
        {
            this._stable = value;
            validIndicator.SetActive(_stable);
            invalidIndicator.SetActive(!_stable);
        }
        get => _stable;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Stable)
        {
            transform.localScale = Vector3.one * (1f+Mathf.Sin(Time.time)/3f);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}