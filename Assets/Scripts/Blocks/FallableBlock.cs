using System.Collections;
using DG.Tweening;
using UnityEngine;

enum BlockState
{
    Static,
    Touched,
    Fall
}

public class FallableBlock : MonoBehaviour, IFallableBlock
{
    private BlockState _blockState;

    [Header("Направление падения")]
    [SerializeField]
    private float _fallDistance;
    [SerializeField]
    private float _fallSpeed;

    [SerializeField]
    private Vector3 _fallRotation;
    [SerializeField]
    private float _rotationSpeed;

    [Header("Задержка между выполнениями действий")]
    [SerializeField]
    private float _fallDelay;
    [SerializeField]
    private float _destroyDelay;

    private void Start()
    {
        _blockState = BlockState.Static;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Enemy" && _blockState == BlockState.Static)
        {
            StartCoroutine(StartDestroy());
        }
    }

    public IEnumerator StartDestroy()
    {
        if(_blockState == BlockState.Static)
        {
            _blockState = BlockState.Touched;
            SetMaterialOnTrigger();

            yield return new WaitForSeconds(_fallDelay);

            _blockState = BlockState.Fall;
            BlockFallDown();

            yield return new WaitForSeconds(_destroyDelay);

            Destroy(gameObject);
        }
    }

    private void BlockFallDown()
    {
        transform.DOMoveY(_fallDistance, _fallSpeed);
        transform.DORotate(_fallRotation, _rotationSpeed);
    }

    private void SetMaterialOnTrigger()
    {
        GetComponent<MeshRenderer>().material.color = Color.red; 
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
