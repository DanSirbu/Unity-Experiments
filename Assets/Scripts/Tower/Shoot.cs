using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour
{
    private float cannonBallTimeToGround = 1.233048f;
    private float cannonBallTimeToGroundSquared = 1.52040816f;

    public GameObject cannonBallPrefab;
    public Transform TowerShootPoint;
    public Rigidbody target;
    public Transform RotationPart;

    public float ShootDelay = 3.0f;

    void Start()
    {
        //spawnPoint = 7.7
        //distanceToGround = 7.7 - 0.25 (ball radius)
        //distanceToGround = 7.45
        //t = sqrt(2*7.45/9.8)

        //need horizontalDistance
        StartCoroutine(ShootCannonball());
    }

    private void Update()
    {
        Vector3 startPoint = TowerShootPoint.transform.position;
        Vector3 endPoint = target.transform.position + (cannonBallTimeToGround * target.velocity);
        endPoint.y = RotationPart.position.y;

        RotationPart.LookAt(endPoint);
    }

    private IEnumerator ShootCannonball()
    {
        while (true)
        {
            Vector3 startPoint = TowerShootPoint.transform.position;
            Vector3 endPoint = target.transform.position + (cannonBallTimeToGround * target.velocity);

            ShootCannon(startPoint, endPoint, target);
            yield return new WaitForSeconds(ShootDelay);
        }
    }

    private void ShootCannon(Vector3 startPoint, Vector3 endPoint, Rigidbody target)
    {
        Vector3 rotationPart = RotationPart.position;
        rotationPart.y = 0;

        if (Vector3.Angle(RotationPart.forward, endPoint - rotationPart) > 5f)
            return;

        GameObject cannonBall = Instantiate(cannonBallPrefab, startPoint, Quaternion.identity) as GameObject;
        Rigidbody cannonBallRigidbody = cannonBall.GetComponent<Rigidbody>();

        Vector3 direction = endPoint - new Vector3(startPoint.x, 0, startPoint.z);
        float horizontalVelocity = direction.magnitude / cannonBallTimeToGround;
        cannonBallRigidbody.velocity = direction.normalized * horizontalVelocity;

        Destroy(cannonBall, ShootDelay);
    }
}