using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepEffect : MonoBehaviour
{
    public GameObject particleLeftFoot, particleRightFoot;
    private ParticleSystem particleLeftFootSystem, particleRightFootSystem;
    void Start()
    {
        particleLeftFootSystem = particleLeftFoot.GetComponent<ParticleSystem>();
        particleRightFootSystem = particleRightFoot.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FootR(){
        particleRightFootSystem.Play();
    }

    public void FootL(){
        particleLeftFootSystem.Play();
    }

    public void Hit() {
        
    }
}
