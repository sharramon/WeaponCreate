using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    [CreateAssetMenu(fileName = "Weapon list", menuName = "ScriptableObjects/FullMetal/Weapon List")]
    public class WeaponList : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> m_weaponList;

        public GameObject GetWeapon(string weaponName)
        {
            foreach(GameObject gameObject in m_weaponList)
            {
                if(gameObject.name == weaponName)
                {
                    return gameObject;
                }
            }

            return null;
        }
    }
}
