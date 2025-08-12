using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameSetup : MonoBehaviour
{

    float ballRadius;
    float ballDiameter;

    [SerializeField] GameObject cueballPrefab;
    [SerializeField] GameObject redballPrefab;
    [SerializeField] GameObject yellowballPrefab;
    [SerializeField] Transform CueBallPos;
    [SerializeField] Transform RedBallPos;
    [SerializeField] Transform YellowBallPos;

     void Awake()
    {
        ballRadius = cueballPrefab.GetComponent<SphereCollider>().radius * 100f;
        ballRadius = redballPrefab.GetComponent<SphereCollider>().radius * 100f;
        ballRadius = yellowballPrefab.GetComponent<SphereCollider>().radius * 100f;

        ballDiameter = ballRadius * 2f;
        PlaceAllBall();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaceAllBall()
    {
        PlaceCueBall();
        PlaceRedBall();
        PlaceYellowBall();
    }

    void PlaceCueBall()
    {
        GameObject ball = Instantiate(cueballPrefab, CueBallPos.position, Quaternion.identity);
        ball.GetComponent<Ball>().MakeCueBall();
    }

    void PlaceRedBall()
    {
        GameObject redBall = Instantiate(redballPrefab, RedBallPos.position, Quaternion.identity);
        Renderer[] renderers = redBall.GetComponentsInChildren<Renderer>();
        redBall.GetComponent<RedBall>().MakeRedBall();
    }

    void PlaceYellowBall()
    {
        GameObject yellowBall = Instantiate(yellowballPrefab, YellowBallPos.position, Quaternion.identity);
        Renderer[] renderers = yellowBall.GetComponentsInChildren<Renderer>();
        yellowBall.GetComponent<YellowBall>().MakeYellowBall();
    }
}
