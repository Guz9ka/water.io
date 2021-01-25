using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooledGameObject
{
	void CopyDataTo(IPooledGameObject pooledObject);
}
