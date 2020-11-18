using UnityEngine;

public class BombBlock : MonoBehaviour
{
    private bool _isExploded = false;
    [SerializeField]
    private Material _activeBombMaterial;
    [SerializeField]
    private float _triggerRadius;
    [SerializeField]
    private float _explosionRadius;

    private void Update()
    {
        TryTriggerBombExplode();
    }

    void TryTriggerBombExplode()
    {
        bool checkSphere = Physics.CheckSphere(transform.position, _triggerRadius, LayerMask.GetMask("Player", "Enemy"));

        if (checkSphere && !_isExploded)
        {
            BombExplode();
        }
    }

    void BombExplode()
    {
        Collider[] fallableBlocks = Physics.OverlapSphere(transform.position, _explosionRadius, LayerMask.GetMask("Ground"));

        foreach (Collider fallableBlock in fallableBlocks)
        {
            IFallableBlock block = fallableBlock.gameObject.GetComponent<IFallableBlock>();
            if (block != null) StartCoroutine(block.StartDestroy());
        }
    }
}
