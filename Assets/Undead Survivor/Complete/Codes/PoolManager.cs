using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goldmetal.UndeadSurvivor
{
    public class PoolManager : MonoBehaviour
    {
        //���� �뵵�� ������
        public GameObject[] prefabs;
        List<GameObject>[] pools;
        //�Ѿ˿뵵�� ������
        public GameObject[] bullet_prefabs;
        List<GameObject>[] bullet_pools;
        void Awake()
        {
            pools = new List<GameObject>[prefabs.Length];

            for (int index = 0; index < pools.Length; index++) {
                pools[index] = new List<GameObject>();
            }
            bullet_pools = new List<GameObject>[bullet_prefabs.Length];

            for (int index = 0; index < bullet_pools.Length; index++)
            {
                bullet_pools[index] = new List<GameObject>();
            }
        }
        public GameObject Get_Bullet(int index)
        {
            GameObject select = null;

            // ����Ʈ�� �����Ͽ� null ��ü ����
            bullet_pools[index].RemoveAll(item => item == null);

            // ��Ȱ��ȭ�� ��ü�� ã�� ����
            foreach (GameObject item in bullet_pools[index])
            {
                if (item != null && !item.activeSelf)
                {
                    select = item;
                    select.SetActive(true);
                    break;
                }
            }

            // ����� ��ü�� ������ ���� ����
            if (!select)
            {
                select = Instantiate(bullet_prefabs[index], transform);
                bullet_pools[index].Add(select);
            }

            return select;
        }
        public GameObject Get_Enemy(int index)
        {
            GameObject select = null;

            // ����Ʈ�� �����Ͽ� null ��ü ����
            pools[index].RemoveAll(item => item == null);

            // ��Ȱ��ȭ�� ��ü�� ã�� ����
            foreach (GameObject item in pools[index])
            {
                if (item != null && !item.activeSelf)
                {
                    select = item;
                    select.SetActive(true);
                    break;
                }
            }

            // ����� ��ü�� ������ ���� ����
            if (!select)
            {
                select = Instantiate(prefabs[index], transform);
                pools[index].Add(select);
            }

            return select;
        }
    }
}
