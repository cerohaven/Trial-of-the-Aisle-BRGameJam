using UnityEngine;

public class HealingEffect : MonoBehaviour
{
    public ParticleSystem healingParticles;

    void Start()
    {
        // You can adjust these parameters as needed
        var main = healingParticles.main;
        main.startColor = Color.green;
        main.startSize = 0.2f;
        main.startLifetime = 2.0f;
        main.startSpeed = 5.0f;
        main.maxParticles = 1000;

        var emission = healingParticles.emission;
        emission.rateOverTime = 500;

        var shape = healingParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1.0f;
    }

    public void ActivateHealingEffect()
    {
        healingParticles.Play();
    }
}
