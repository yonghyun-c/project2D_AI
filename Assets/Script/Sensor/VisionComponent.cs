using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionComponent : MonoBehaviour {
    private static readonly Color DETECT_COLOR = new Color(155f, 0f, 0f, .4f);
    private static readonly Color RELEASE_COLOR = new Color(155f, 155f, 155f, .4f);

    SpriteRenderer m_SpriteRenderer;
    Vector3? position;

    // Use this for initialization
    void Start () {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.color = RELEASE_COLOR;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Runner"))
        {
            position = other.transform.position;
            m_SpriteRenderer.color = DETECT_COLOR;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Runner"))
        {
            position = null;
            m_SpriteRenderer.color = RELEASE_COLOR;
        }
    }

    public Vector3 GetTargetLocation()
    {
        return position.HasValue ? position.Value : Vector3.zero;
    }
    
    public bool IsObserving()
    {
        return position.HasValue;
    }
}
