using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helper for applying actions to various particle systems in a particle effect
public class ParticleSystemsHelper : MonoBehaviour
{

    public List<ParticleSystem> particleSystems;

    protected Transform _userTransform;
    protected Transform _targetTransform;

    protected int _orderInLayer = 0;
    protected float _destroyDuration = 5f;
    
    public virtual void SetRenderOrder(int orderInLayer) {
        this._orderInLayer = orderInLayer;

        foreach (ParticleSystem system in particleSystems) {
            ParticleSystemRenderer pr = (ParticleSystemRenderer)system.GetComponent<Renderer>();
            pr.sortingOrder = orderInLayer;
        }
    }

    public void SetDestroyDuration(float time) {
        this._destroyDuration = time;

        DestroyAfterSeconds destroyAfterSeconds = this.GetComponent<DestroyAfterSeconds>();
        if (destroyAfterSeconds != null) {
            destroyAfterSeconds.duration = time;
        }
    }


    public virtual void Play() {
        foreach (ParticleSystem system in particleSystems) {
            system.Play();
        }
    }


    public virtual void Stop() {
        foreach (ParticleSystem system in particleSystems) {
            system.Stop();
        }
    }

    public virtual void Clear() {
        foreach (ParticleSystem system in particleSystems) {
            system.Clear();
        }
    }

    public virtual void PositionEffect(Transform userTransform, Transform targetTransform) {
        this._userTransform = userTransform;
        this._targetTransform = targetTransform;

        this.transform.position = this._targetTransform.position;
    }
}
