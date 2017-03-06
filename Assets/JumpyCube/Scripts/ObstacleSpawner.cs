using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    private GameObject _obstable;
    public float openingSize = 0;

    private GameObject generated;

    void Start()
    {
        _obstable = Resources.Load("Prefabs/Wall") as GameObject;
    }

    public void Generate()
    {
        float position = Random.Range(2, 9);

        GameObject obstacleGameObject = Instantiate(_obstable, new Vector3(20, 0, 0), Quaternion.identity) as GameObject;
        generated = obstacleGameObject;
        var obstacle = obstacleGameObject.GetComponent<Wall>();
        obstacle.velocity = 5.0f;
        obstacle.verticalPosition = position;
        obstacle.openingSize = openingSize;
        obstacle.Enable();

    }

    public void StartScript()
    {
        Generate();
    }

    public void StopScript()
    {
        if (generated != null)
            generated.transform.position = new Vector3(0, -9999, 0);
    }
}
