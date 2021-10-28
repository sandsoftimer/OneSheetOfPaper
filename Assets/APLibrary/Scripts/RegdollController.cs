using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegdollController : APBehaviour
{
    public Transform regdollRigRoot;
    public bool activeOnAwake;
    public SkinnedMeshRenderer[] skins;
    public List<Rigidbody> rigs;
    public List<Collider> regdollParts;

    Animator anim;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();
        SetRegdollParts();
        SetRandomSkin();

        if(activeOnAwake)
            TurnOnRegdoll();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    #endregion ALL UNITY FUNCTIONS
    //=================================   
    #region ALL OVERRIDING FUNCTIONS


    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS

    void SetRegdollParts()
    {
        regdollParts = new List<Collider>();
        Collider[] regdollColliders = regdollRigRoot.GetComponentsInChildren<Collider>();

        foreach (Collider item in regdollColliders)
        {
            item.isTrigger = true;
            regdollParts.Add(item);
            Rigidbody r = item.GetComponent<Rigidbody>();
            r.isKinematic = true;
            rigs.Add(r);

        }
    }

    public void TurnOnRegdoll()
    {
        //anim.enabled = false;
        int index = 0;
        foreach (Collider item in regdollParts)
        {
            item.isTrigger = false;
            //rigs[index].velocity = rig.velocity;
            rigs[index].isKinematic = false;
            index++;
        }

    }

    void SetRandomSkin()
    {
        int skinIndex = Random.Range(0, skins.Length);
        for (int i = 0; i < skins.Length; i++)
        {
            skins[i].gameObject.SetActive(i == skinIndex);
        }
    }

    #endregion ALL SELF DECLEAR FUNCTIONS

}
