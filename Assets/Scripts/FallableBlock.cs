using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BlockState
{
    Static,
    Touched,
    Fall
}

public class FallableBlock : MonoBehaviour
{
    private BlockState blockState;

    [Header("Задержка между выполнениями действий")]
    public float fallDelay;
    public float destroyDelay;

    [Header("Скорость падения")]
    public float fallSpeed;

    private void Start()
    {
        blockState = BlockState.Static;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "Enemy" && blockState == BlockState.Static)
        {
            blockState = BlockState.Touched;
            StartCoroutine(StartDestroy());
        }
    }

    private IEnumerator StartDestroy()
    {
        //активировать визуальный эффект

        yield return new WaitForSeconds(fallDelay);

        blockState = BlockState.Fall;

        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }

    private void Update()
    {
        if(blockState == BlockState.Fall) 
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, Mathf.Lerp(gameObject.transform.position.y, 0, fallSpeed * Time.deltaTime), gameObject.transform.position.z);
        }
    }
}
