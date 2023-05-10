using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CustomPrefabPool : MonoBehaviour, IPunPrefabPool
{
    public Dictionary<string, Queue<GameObject>> PrefabPool = new Dictionary<string, Queue<GameObject>>();

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject obj;

        if (PrefabPool.ContainsKey(prefabId) && PrefabPool[prefabId].Count > 0)
        {
            obj = PrefabPool[prefabId].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        else
        {
            obj = PhotonNetwork.Instantiate(prefabId, position, rotation);
        }

        return obj;
    }

    public void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false);

        if (!PrefabPool.ContainsKey(gameObject.name))
        {
            PrefabPool[gameObject.name] = new Queue<GameObject>();
        }

        PrefabPool[gameObject.name].Enqueue(gameObject);
    }
}
