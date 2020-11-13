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
    private BlockState blockState;

    [Header("Задержка между выполнениями действий")]
    [SerializeField]
    private float fallDelay;
    [SerializeField]
    private float destroyDelay;

    [Header("Скорость падения")]
    [SerializeField]
    private float fallSpeed;

    private void Start()
    {
        blockState = BlockState.Static;
    }

    private void FixedUpdate()
    {
        if (blockState == BlockState.Fall)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, Mathf.Lerp(gameObject.transform.position.y, 0, fallSpeed * Time.deltaTime), gameObject.transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Enemy" && blockState == BlockState.Static)
        {
            StartCoroutine(StartDestroy());
        }
    }

    public IEnumerator StartDestroy()
    {
        if(blockState == BlockState.Static)
        {
            blockState = BlockState.Touched;
            SetMaterialOnTrigger();

            yield return new WaitForSeconds(fallDelay);

            blockState = BlockState.Fall;

            yield return new WaitForSeconds(destroyDelay);

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
