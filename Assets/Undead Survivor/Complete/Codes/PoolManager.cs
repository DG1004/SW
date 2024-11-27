using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goldmetal.UndeadSurvivor
{
    public class PoolManager : MonoBehaviour
    {
        public GameObject[] prefabs;

        List<GameObject>[] pools;

        void Awake()
        {
            pools = new List<GameObject>[prefabs.Length];

            for (int index = 0; index < pools.Length; index++) {
                pools[index] = new List<GameObject>();
            }
        }
   
        public GameObject Get(int index)
        {
            GameObject select = null;

            // 리스트를 정리하여 null 객체 제거
            pools[index].RemoveAll(item => item == null);

            // 비활성화된 객체를 찾아 재사용
            foreach (GameObject item in pools[index])
            {
                if (item != null && !item.activeSelf)
                {
                    select = item;
                    select.SetActive(true);
                    break;
                }
            }

            // 사용할 객체가 없으면 새로 생성
            if (!select)
            {
                select = Instantiate(prefabs[index], transform);
                pools[index].Add(select);
            }

            return select;
        }
    }
}
