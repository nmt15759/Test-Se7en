using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KickManager : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] List<Transform> balls;        
    [SerializeField] Transform[] goalTargets;      // Goal_Left, Goal_Right
    [SerializeField] CameraFollow cameraFollow;

    [SerializeField] Button kickButton;
    [SerializeField] Button autoKickButton;
    [SerializeField] Button resetButton;

    [SerializeField] float kickRange = 2f;         
    [SerializeField] float flightTime = 1.2f;      
    [SerializeField] float arcHeight = 3f;         
    [SerializeField] float cameraReturnDelay = 2f; 
    [SerializeField] GameObject confettiPrefab;

    bool isFlying = false;

    void Start()
    {
        kickButton.onClick.AddListener(OnKickClick);
        autoKickButton.onClick.AddListener(OnAutoKickClick);
        resetButton.onClick.AddListener(OnResetClick);
    }

    void Update()
    {
        Transform nearBall = FindBall(true);
        bool canKick = !isFlying && nearBall != null
                       && Vector3.Distance(player.position, nearBall.position) <= kickRange;
        kickButton.gameObject.SetActive(canKick);

        autoKickButton.interactable = !isFlying && FindBall(false) != null;
    }

    void OnKickClick()
    {
        Transform ball = FindBall(true);   
        if (ball != null) StartCoroutine(KickBall(ball));
    }

    void OnAutoKickClick()
    {
        Transform ball = FindBall(false); 
        if (ball != null) StartCoroutine(KickBall(ball));
    }

    void OnResetClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator KickBall(Transform ball)
    {
        isFlying = true;

        Vector3 start = ball.position;
        Vector3 end = FindNearestGoal(start).position;
        cameraFollow.SetTarget(ball); 

        
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flightTime;
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
            ball.position = pos;
            yield return null;
        }
        ball.position = end;
        balls.Remove(ball);   

      
        if (confettiPrefab != null)
        {
            GameObject fx = Instantiate(confettiPrefab, end, Quaternion.identity);
            fx.GetComponent<ParticleSystem>().Play();
            Destroy(fx, 8f);
        }

        
        yield return new WaitForSeconds(cameraReturnDelay);
        cameraFollow.SetTarget(player);
        isFlying = false;
    }

  
    Transform FindBall(bool nearest)
    {
        Transform result = null;
        float best = nearest ? float.MaxValue : 0f;

        foreach (Transform ball in balls)
        {
            float d = Vector3.Distance(player.position, ball.position);
            if (nearest && d < best) { best = d; result = ball; }
            if (!nearest && d > best) { best = d; result = ball; }
        }
        return result;
    }

   
    Transform FindNearestGoal(Vector3 from)
    {
        Transform result = goalTargets[0];
        float best = float.MaxValue;

        foreach (Transform goal in goalTargets)
        {
            float d = Vector3.Distance(from, goal.position);
            if (d < best) { best = d; result = goal; }
        }
        return result;
    }
}