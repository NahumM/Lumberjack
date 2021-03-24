using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavCharacterController : MonoBehaviour
{
    public static NavCharacterController instance;

    NavMeshAgent agent;

    Animator anim;
    [SerializeField] AnimationClip axeMineAnim;
    [SerializeField] AnimationClip gatheringAnim;

    List<Wood> woodToCut = new List<Wood>();
    [SerializeField] List<GameObject> trunksOnBack = new List<GameObject>();
    [HideInInspector] public List<GameObject> trunksToPickUp = new List<GameObject>();
    Wood closestWood;

    enum States { Idle, GoingToCut, GoingToPickUp, GoingHome, WaitingForTrees};
    States characterState;

    [SerializeField] GameObject home;

    private void Awake() => instance = this;

    private void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        FindTreeToCut();
    }

    public void AddWoodToCut(Wood wood)
    {
        woodToCut.Add(wood);
        if (characterState == States.WaitingForTrees)
            FindTreeToCut();
    }
    public void RemoveWoodToCut(Wood wood) => woodToCut.Remove(wood);

    public void FindTreeToCut() //Finding the closest tree, if there is none, start waiting for new one.
    {
        closestWood = null;
        if (woodToCut.Count != 0)
        {
            foreach (Wood i in woodToCut)
            {
                Vector3 woodPosistion = i.treeGameObject.transform.position;

                if (closestWood == null)
                    closestWood = i;

                float distanceToTree = Vector3.Distance(transform.position, woodPosistion);

                if (Vector3.Distance(closestWood.treeGameObject.transform.position, transform.position) > distanceToTree)
                    closestWood = i;
            }
            agent.destination = closestWood.treeGameObject.transform.position;
            characterState = States.GoingToCut;
        }
        else characterState = States.WaitingForTrees;
    }

    private void Update() // Checks if destanation reached according to character state of action.
    {
        if (characterState == States.GoingToCut && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (Vector3.Distance(closestWood.treeGameObject.transform.position, transform.position) < 1f) // If tree for some reason changed its position while character running, this code checks it.
            {
                StartCoroutine("WoodChoping");
                characterState = States.Idle;
            }
            else agent.destination = closestWood.treeGameObject.transform.position;
        }
        if (characterState == States.GoingToPickUp && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine("PickUpTrunk");
            characterState = States.Idle;
        }
        if (characterState == States.GoingHome && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine("CameHome");
            characterState = States.Idle;
        }

        if (agent.velocity.magnitude > 2) // Simple code to activate running animation
        {
            anim.SetBool("isMoving", true);
        }
        else anim.SetBool("isMoving", false);
    }

    IEnumerator WoodChoping()
    {
        for (int i = 0; i < 3; i++)
        {
            anim.SetBool("isChopping", true);
            yield return new WaitForSeconds(axeMineAnim.length);
            closestWood.TreeHit();
        }

        agent.destination = trunksToPickUp[0].transform.position;
        characterState = States.GoingToPickUp;
        anim.SetBool("isChopping", false);
    }

    IEnumerator PickUpTrunk()
    {
        anim.SetBool("isGathering", true);
        yield return new WaitForSeconds(gatheringAnim.length);
        trunksToPickUp[0].SetActive(false);
        trunksToPickUp.RemoveAt(0);
        foreach (GameObject backTrunk in trunksOnBack)
        {
            if (backTrunk.activeSelf == false)
            {
                backTrunk.SetActive(true);
                break;
            }
        }
        anim.SetBool("isGathering", false);
        yield return new WaitForSeconds(gatheringAnim.length);
        if (trunksToPickUp.Count != 0)
        {
            StartCoroutine("PickUpTrunk");
        }
        else
        {
            agent.destination = home.transform.position;
            characterState = States.GoingHome;
        }
    }

    IEnumerator CameHome()
    {
        anim.SetBool("isGathering", true);
        yield return new WaitForSeconds(gatheringAnim.length);
        anim.SetBool("isGathering", false);
        foreach (GameObject trunk in trunksOnBack)
            trunk.SetActive(false);
        FindTreeToCut();
    }


}
