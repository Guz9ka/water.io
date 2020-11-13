using UnityEngine;

public class BombBlock : MonoBehaviour
{
    [SerializeField]
    private Material activeBombMaterial;
    [SerializeField]
    private float triggerRadius;
    [SerializeField]
    private float explosionRadius;

    private void Update()
    {
        TryTriggerBombExplode();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, triggerRadius);
    }

    void TryTriggerBombExplode()
    {
        bool checkSphere = Physics.CheckSphere(transform.position, triggerRadius, LayerMask.GetMask("Player", "Enemy"));
        Debug.Log(checkSphere);
        if (checkSphere)
        {
            BombExplode();
        }
    }

    void BombExplode()
    {
        Collider[] fallableBlocks = Physics.OverlapSphere(transform.position, explosionRadius, LayerMask.GetMask("Ground"));

        foreach (Collider fallableBlock in fallableBlocks)
        {
            IFallableBlock block = fallableBlock.gameObject.GetComponent<IFallableBlock>();
            if (block != null) StartCoroutine(block.StartDestroy());
        }
    }
}
