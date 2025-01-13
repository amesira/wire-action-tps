using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public GameObject explosionPrefab;

	private void Awake() {
        if(instance == null) {
            instance = this;
        }
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetExplosion(Vector3 _pos,Color _color) {
        _color.a = 0.6f;
        GameObject explosion = Instantiate(explosionPrefab, _pos, Quaternion.identity);
        explosion.GetComponent<Explosion>().StartEffect(_color);
    }
}
