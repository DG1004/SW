using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goldmetal.UndeadSurvivor
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]
    public class ItemData : ScriptableObject
    {
        public enum ItemType { Melee, Range, Glove, Shoe, Heal,rare1,rare2, rare3, rare4, newGun }

        [Header("# Main Info")]
        public ItemType itemType;
        public int itemId;
        public string itemName;
        public int itemPrice;

        [TextArea]
        public string itemDesc;
        public Sprite itemIcon;

        [Header("# Level Data")]
        public float baseDamage;
        public int baseCount;
        public float[] damages;
        public int[] counts;

        [Header("# Weapon")]
        public GameObject projectile;
        public Sprite hand;
    }
}
