using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSimRenderer : MonoBehaviour
{
    public CarSimFMI carsim_FMI;

    void Start()
    {
        carsim_FMI = transform.GetComponent<CarSimFMI>();
    }

    void Update()
    {
        transform.position = carsim_FMI.position;
    }
}
