using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abstraction;
using UnityEngine;

public class Bomb : MonoBehaviour, IStaticUnit
{
    [SerializeField] private readonly float _bombDamage = 40f;

    private readonly Dictionary<string, IAliveUnit> _radiusUnits = new Dictionary<string, IAliveUnit>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Boom());
    }

    // Update is called once per frame

    public void Kill()
    {
        var parent = transform.parent;
        BombSpawner.BombsCount--;
        if(parent != null)
            Destroy(parent.gameObject);
        else
            Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var aliveUnit = other.GetComponent<IAliveUnit>();

        if (aliveUnit != null && !_radiusUnits.TryGetValue(aliveUnit.ToString(), out IAliveUnit unit))
        {
            Debug.Log(aliveUnit.ToString());
            _radiusUnits.Add(aliveUnit.ToString(), aliveUnit);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var aliveUnit = other.GetComponent<IAliveUnit>();

        if (aliveUnit != null)
            _radiusUnits.Remove(aliveUnit.ToString());
    }

    public void ToInteract(IAliveUnit unit)
    {
        unit.GetDamage(_bombDamage);
    }

    IEnumerator Boom()
    {
        yield return new WaitForSeconds(5f);
        var itemsToDamage = _radiusUnits.Values.ToList();
        
        for (var i = 0; i < itemsToDamage.Count; i++)
        {
            ToInteract(itemsToDamage[i]);
        }

        var animObj = transform.parent ? transform.parent : transform;
        var timeToKill = animObj.GetComponent<Animation>().clip.length * 2f;
        animObj.GetComponent<Animation>().Play();
        
        yield return new WaitForSeconds(timeToKill);
        Kill();
    }
}
