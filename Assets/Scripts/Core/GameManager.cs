using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Simulation Game = null;

    [Header("UI")]
    public Text Position;
    public Text Score;
    public Text FinalScore;
    public GameObject EndScreen;

    [Header("Player")]
    public Transform PlayerShip;

    [Header("Prefabs")]
    public GameObject EnemyShip;
    public GameObject LaserPref;
    public GameObject BulletPref;
    public GameObject DeadSmallAsteroidPref;
    public GameObject SmallAsteroidPref;
    public GameObject BigAsteroidPref;

    //-------------------------------
    // Debug
    //-------------------------------
    [Header("Other Game Objects")]
    private Transform EnemyShipPos;
    private List<Transform> BulletsPos = new List<Transform>();
    private List<Transform> AsteroidsPos = new List<Transform>();

    public void StartNewGame()
    {
        Game = new Simulation();

        if (EnemyShipPos)
            Destroy(EnemyShipPos.gameObject);
        foreach (var item in BulletsPos)
            Destroy(item.gameObject);
        foreach (var item in AsteroidsPos)
            Destroy(item.gameObject);

        BulletsPos.Clear();
        AsteroidsPos.Clear();
        EndScreen.SetActive(false);

        Game.GameStart();

        //Subscribe on Game Invokes
        Game.OnGameUi += OnGameUi;
        Game.OnNewEnemy += OnNewEnemy;
        Game.OnDeadEnemy += OnDeadEnemy;
        Game.OnDeadBullet += OnDeadBullet;
        Game.OnNewAsteroid += OnNewAsteroid;
        Game.OnDeadAsteroid += OnDeadAsteroid;
        Game.PlayerShip.OnShot += OnNewBullet;
        Game.PlayerShip.OnLaserShot += OnNewLaser;
    }
    void FixedUpdate()
    {
        if (Game == null)
            return;
        
        Game.GameTick(Time.fixedDeltaTime);

        //Выставить позицию своего корабля
        PlayerShip.position = Game.PlayerShip.GetPosition();
        PlayerShip.rotation = Quaternion.Euler(0, 0, Game.PlayerShip.GetRotation());

        //Выставить позицию вражесского корабля
        if (EnemyShipPos && Game.EnemyShip != null)
            EnemyShipPos.position = Game.EnemyShip.GetPosition();


        //Выставить позицию каждой пуле
        for (int i = 0; i < BulletsPos.Count; i++)
        {
            BulletsPos[i].position = Game.Bullets[i].GetPosition();
            BulletsPos[i].rotation = Quaternion.Euler(0, 0, Game.Bullets[i].GetRotation());
        }

        //Выставить позицию каждому астероиду
        for (int i = 0; i < AsteroidsPos.Count; i++)
        {
            AsteroidsPos[i].position = Game.Asteroids[i].GetPosition();
            AsteroidsPos[i].rotation = Quaternion.Euler(0, 0, Game.Asteroids[i].GetRotation());
        }
    }

    //-------------------------------
    // Invokes
    //-------------------------------
    private void OnNewLaser()
    {
        GameObject NewLaser = Instantiate(LaserPref, transform);
        NewLaser.transform.position = PlayerShip.position;
        NewLaser.transform.rotation = PlayerShip.rotation;
    }

    private void OnGameUi(int score, Vector2 pos, float rot, float spd, int health, int lasershots, float laserDelay, bool GameActive)
    {
        Position.text = $"Pos: {pos.x.ToString("0000")} : {pos.y.ToString("0000")}  Rot: {rot.ToString("000")}  Spd: {Mathf.Round(spd)}";
        Score.text = $"SCORE: {score}\nHEALTH: {health}\nLASERS: {lasershots}\nLASER RELOAD: {Mathf.Round(laserDelay)}";

        if (!GameActive)
        {
            EndScreen.SetActive(true);
            FinalScore.gameObject.SetActive(true);
            FinalScore.text = $"YOUR SCORE: {score}";
        }

    }

    private void OnNewEnemy()
    {
        Debug.Log("New Enemy Ship");

        EnemyShipPos = Instantiate(EnemyShip, transform).transform;
    }
    private void OnDeadEnemy()
    {
        Debug.Log("Destroyed Enemy Ship");

        Destroy(EnemyShipPos.gameObject);
    }

    private void OnNewBullet()
    {
        Debug.Log("New Bullet");
        GameObject NewBullet;
        NewBullet = Instantiate(BulletPref, transform);
        BulletsPos.Add(NewBullet.transform);
    }
    private void OnDeadBullet(int id)
    {
        Destroy(BulletsPos[id].gameObject);
        BulletsPos.RemoveAt(id);
    }

    private void OnNewAsteroid(bool Big)
    {
        GameObject NewAsteroid;
        if (Big)
        {
            Debug.Log("New Big Asteroid");
            NewAsteroid = Instantiate(BigAsteroidPref, transform);
        }
        else
        {
            Debug.Log("New Small Asteroid");
            NewAsteroid = Instantiate(SmallAsteroidPref, transform);
        }

        AsteroidsPos.Add(NewAsteroid.transform);
    }
    private void OnDeadAsteroid(int id)
    {
        Debug.Log("Asteroid Destroyed");
        GameObject DeadAsteroid = Instantiate(DeadSmallAsteroidPref, transform);
        DeadAsteroid.transform.position = AsteroidsPos[id].position;
        DeadAsteroid.transform.rotation = AsteroidsPos[id].rotation;

        Destroy(AsteroidsPos[id].gameObject);
        AsteroidsPos.RemoveAt(id);
    }
}
