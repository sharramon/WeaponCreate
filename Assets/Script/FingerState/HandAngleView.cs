using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FullMetal
{
    public class HandAngleView : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_distanceText;
        [SerializeField] private WeaponCreate m_weaponCreate;

        // Update is called once per frame
        void Update()
        {
            UpdateDistanceText();
        }

        private void UpdateDistanceText()
        {
            m_distanceText.text = GetDistance().ToString();
        }

        private float GetDistance()
        {
            float angle = m_weaponCreate._angle;
            angle = Mathf.Round(angle * 100f) / 100f;
            return angle;
        }
    }
}
