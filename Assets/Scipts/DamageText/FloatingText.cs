using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private int damage;
    [SerializeField] private GameObject _floatingTextPrefab;
    // Start is called before the first frame update
    void Start()
    {
        damage = Random.Range(0, 100);
        ShowDamage(damage.ToString());
    }

    void ShowDamage(string text)
    {
        if (_floatingTextPrefab)
        {
            GameObject prefab = Instantiate(_floatingTextPrefab, transform.position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMesh>().text = text;
        }
    }

    
}
