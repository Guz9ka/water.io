using System.Collections;
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

    [Header("Задержка между выполнениями действий")]
    [SerializeField]
    private float _fallDelay;
    [SerializeField]
    private float _destroyDelay;

    [Header("Скорость падения")]
    [SerializeField]
    private float _fallSpeed;

    private void Start()
    {
        _blockState = BlockState.Static;
    }

    private void FixedUpdate()
    {
        if (_blockState == BlockState.Fall)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, Mathf.Lerp(gameObject.transform.position.y, 0, _fallSpeed * Time.deltaTime), gameObject.transform.position.z);
        }
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

            yield return new WaitForSeconds(_destroyDelay);

            Destroy(gameObject);
        }
    }

    private void SetMaterialOnTrigger()
    {
        GetComponent<MeshRenderer>().material.color = Color.green; 
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
