using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class particlehandler : MonoBehaviour
{
    // Start is called before the first frame update
    public VisualEffect[] driftParts;
    public ParticleSystem[] thrusterFlames;
    public GameObject[] thrusterRef;

    [ColorUsage(true, true)]
    public Color earlyDrift;

    [ColorUsage(true, true)]
    public Color oldDrift;

    [ColorUsage(true, true)]
    public Color bestDrift;

    bool partsEnabled = false;
    VehicleMovement vm;

    void Start()
    {
        vm = GetComponent<VehicleMovement>();
        EndDrift();
    }

    // Update is called once per frame
    void Update()
    {
        if (vm.isDrifting && partsEnabled == false)
        {
            StartDrift();
        }
        else if (!vm.isDrifting)
        {
            EndDrift();
        }
        UpgradeDrift();
        FlameSize();
    }

    void StartDrift()
    {
        foreach (VisualEffect effect in driftParts)
        {
            effect.Play();
        }
        partsEnabled = true;
    }

    void EndDrift()
    {
        foreach (VisualEffect effect in driftParts)
        {
            effect.Stop();
        }
        partsEnabled = false;
    }

    void UpgradeDrift()
    {
        foreach (VisualEffect effect in driftParts)
        {
            if (Time.time - vm.driftStart < vm.driftTimes[0])
            {
                effect.SetVector4("Color", earlyDrift);
            }
            else if(Time.time - vm.driftStart > vm.driftTimes[0] && Time.time - vm.driftStart < vm.driftTimes[1])
            {
                effect.SetVector4("Color", oldDrift);
            }
            else if (Time.time - vm.driftStart > vm.driftTimes[1] && vm.isDrifting)
            {
                effect.SetVector4("Color", bestDrift);
            }
        }
    }

    void FlameSize()
    {
        foreach (ParticleSystem particle in thrusterFlames)
        {
            particle.startSpeed = vm.curSpeed/20;
            foreach (GameObject thrusterMat in thrusterRef)
            {
                thrusterMat.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, ((vm.maxSpeed - vm.curSpeed)/ vm.maxSpeed)+0.3f);
            }
        }
    }
}
