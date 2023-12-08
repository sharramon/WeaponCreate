using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullMetal
{
    
    public class PalmTrigger : MonoBehaviour
    {
        [SerializeField] private List<string> m_tags;
        [SerializeField] private bool m_isLeft;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"On trigger entered with {other.gameObject.name}");
            foreach (string tag in m_tags)
            {
                if (other.gameObject.tag == tag)
                {
                    TagTouchEvent(tag, other.gameObject);
                    Debug.Log($"Tag event occured with tag {tag}");
                    return;
                }
            }
        }

        private void TagTouchEvent(string tag, GameObject backObject)
        {
            EventManager.Instance._tagTouchedEvent?.Invoke(tag, m_isLeft, backObject);
        }
    }
}
