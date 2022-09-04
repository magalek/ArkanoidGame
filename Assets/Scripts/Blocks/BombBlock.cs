using UnityEditor;
using UnityEngine;

public class BombBlock : SimpleBlock {

    [SerializeField, Range(0, 5f)] private float explosionRadius;

    private Collider2D[] buffer = new Collider2D[10];

    public override void Destroy() {
        base.Destroy();
        int bufferLength = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, buffer);
        for (int i = 0; i < bufferLength; i++) {
            if (buffer[i].TryGetComponent(out IDamageable damageable) && damageable.GetType() != typeof(BombBlock)) damageable.Destroy();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
