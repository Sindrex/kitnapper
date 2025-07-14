using System.Collections.Generic;
using UnityEngine;

namespace ASCII.Util
{
    public static class GameObjectUtil
    {
        public static void DestroyChildren(GameObject go)
        {
            var removeList = new List<GameObject>();
            foreach (Transform child in go.transform)
            {
                removeList.Add(child.gameObject);
            }
            for(int i = removeList.Count - 1; i >= 0; i--)
            {
                GameManager.Destroy(removeList[i]);
            }
        }

        public static void DestroyMyChildren(this GameObject go)
        {
            var removeList = new List<GameObject>();
            foreach (Transform child in go.transform)
            {
                removeList.Add(child.gameObject);
            }
            for(int i = removeList.Count - 1; i >= 0; i--)
            {
                GameManager.Destroy(removeList[i]);
            }
        }
    }
}
