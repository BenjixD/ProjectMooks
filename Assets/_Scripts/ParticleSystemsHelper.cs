using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helper for applying actions to various particle systems in a particle effect
public class ParticleSystemsHelper : MonoBehaviour
{

    public List<ParticleSystem> particleSystems;
    
    public void SetRenderOrder(int orderInLayer) {
        foreach (ParticleSystem system in particleSystems) {
            ParticleSystemRenderer pr = (ParticleSystemRenderer)system.GetComponent<Renderer>();
            pr.sortingOrder = orderInLayer;
        }
    }
}
