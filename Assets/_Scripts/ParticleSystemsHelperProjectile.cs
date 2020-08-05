using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helper for applying actions to various particle systems in a particle effect
public class ParticleSystemsHelperProjectile : ParticleSystemsHelper
{

    public float projectileMoveTime = 1f;

    public ParticleSystemsHelper explosionPrefab;
    
    public override void Play() {
        base.Play();

        StartCoroutine(this.AnimateToTarget());
    }

    public override void PositionEffect(Transform userTransform, Transform targetTransform) {
        base.PositionEffect(userTransform, targetTransform);
        this.transform.position = this._userTransform.position;
    }

    private IEnumerator AnimateToTarget() {
        float counter = 0;

        while (counter < this.projectileMoveTime) {
            float t = counter / projectileMoveTime;

            Vector3 projectilePos = Vector3.Lerp(this._userTransform.position, this._targetTransform.position, t);  // May want a more refined lerp in the future
            this.transform.position = projectilePos;
            counter += GameManager.Instance.time.deltaTime;

            yield return null;
        }

        base.Clear();
        base.Stop();
        ParticleSystemsHelper psh = Instantiate(explosionPrefab, this._targetTransform);
        // hitEffect.SetDestroyDuration(5f); // maybe let the animation handle this?
        psh.SetRenderOrder(this._orderInLayer);
        psh.PositionEffect(this._userTransform, this._targetTransform);
        psh.Play();
    }
}
