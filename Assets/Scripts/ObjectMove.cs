using UnityEngine;
using System.Collections;

public class ObjectMove : MonoBehaviour
{
    public float size;
    public Vector3 moveSpeed;
    private Vector3 destroyPos;
    private GameObject parent;
    private ObjectsMoveManager parentManager;
    public Vector3 resize;
    private bool grown = false;
    private bool calledSpawn = false;

    public GameManager gameManager;

    // Chỉ random theo Y và Z
    [Header("Random Settings")]
    public Vector2 randomYPos = new Vector2(-2f, 2f);
    public Vector2 randomZPos = new Vector2(-1f, 1f);

    public Vector2 randomScale = new Vector2(0.5f, 2f);
    public Vector2 randomRotationY = new Vector2(0f, 360f);
    public Vector2 randomRotationZ = new Vector2(0f, 360f);

    void Start()
    {
        gameManager = Object.FindObjectOfType<GameManager>();
        parent = this.transform.parent.gameObject;
        parentManager = parent.GetComponent<ObjectsMoveManager>();
        destroyPos = parentManager.destroyPos;

        // Random vị trí theo Y và Z, giữ nguyên X
        Vector3 localPos = transform.localPosition;
        localPos.y += Random.Range(randomYPos.x, randomYPos.y);
        localPos.z += Random.Range(randomZPos.x, randomZPos.y);
        transform.localPosition = localPos;

        // Random scale (nếu cần)
        float randScale = Random.Range(randomScale.x, randomScale.y);
        transform.localScale = new Vector3(randScale, randScale, randScale);

        // Random rotation chỉ theo Y và Z
        transform.localRotation = Quaternion.Euler(
            0f,
            Random.Range(randomRotationY.x, randomRotationY.y),
            Random.Range(randomRotationZ.x, randomRotationZ.y)
        );
    }

    void ResizeUpCheck()
    {
        if (this.transform.localScale.x >= 1.0f)
        {
            grown = true;
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            ResizeUp();
        }
    }

    void ResizeDownCheck()
    {
        //Resize object down
        if (this.transform.localPosition.x <= destroyPos.x)
        {
            if (this.transform.localScale.x <= 0)
                Destroy(this.gameObject);
            else
                ResizeDown();
        }
    }

    void ResizeUp()
    {
        this.transform.localScale += resize * Time.deltaTime;
    }

    void ResizeDown()
    {
        this.transform.localScale -= resize * Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (gameManager.startedGame && gameManager.stillAlive)
        {
            //Move object
            transform.localPosition += moveSpeed * Time.deltaTime;

            //Check if spawn another
            if (this.transform.localPosition.x < (size * parentManager.spawnFrequency * -1) && calledSpawn == false)
            {
                StartCoroutine(DelayedSpawn());
                calledSpawn = true;
            }

            //Resize check
            if (grown == false)
            {
                ResizeUpCheck();
            }
            else
            {
                ResizeDownCheck();
            }
        }
    }

    IEnumerator DelayedSpawn()
    {
        float waitTime = Random.Range(0.5f, 2f); // random từ 0.5s đến 2s
        yield return new WaitForSeconds(waitTime);
        parentManager.SpawnCall();
    }

}
