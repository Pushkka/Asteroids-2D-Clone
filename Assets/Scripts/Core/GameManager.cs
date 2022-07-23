using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Core Game = null;

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
    public GameObject SmallAsteroidPref;
    public GameObject BigAsteroidPref;

    //-------------------------------
    // Debug
    //-------------------------------
    [Header("Other Game Objects")]
    private Transform EnemyShipPos;
    private List<Transform> BulletsPos;
    private List<Transform> SmallAsteroidsPos;
    private List<Transform> BigAsteroidsPos;

    public void StartNewGame()
    {
        //Create new Core
        Game = new Core();

        //Destroy old objects
        if (EnemyShipPos)
            Destroy(EnemyShipPos.gameObject);
        foreach (var item in BulletsPos)
            Destroy(item.gameObject);
        foreach (var item in SmallAsteroidsPos)
            Destroy(item.gameObject);
        foreach (var item in BigAsteroidsPos)
            Destroy(item.gameObject);

        BulletsPos.Clear();
        SmallAsteroidsPos.Clear();
        BigAsteroidsPos.Clear();
        EndScreen.SetActive(false);

        //Subscribe on Game Invokes
        Game.OnNewEnemy += Game_OnNewEnemy;
        Game.OnDeadEnemy += Game_OnDeadEnemy;
        Game.OnNewBullet += Game_OnNewBullet;
        Game.OnNewLaser += Game_OnNewLaser; ;
        Game.OnDeadBullet += Game_OnDeadBullet;
        Game.OnNewAsteroid += Game_OnNewAsteroid;
        Game.OnDeadAsteroid += Game_OnDeadAsteroid;
        Game.OnGameUi += Game_OnGameUi;

        Game.GameStart();
    }
    void FixedUpdate()
    {
        //Check Game Core
        if (Game == null)
            return;

        //Apply game tick
        Game.GameTick(Time.fixedDeltaTime);

        //Set Player ship position
        PlayerShip.position = Game.PlayerShip.GetPosition();
        PlayerShip.rotation = Quaternion.Euler(0, 0, Game.PlayerShip.GetRotation());

        //Set Enemy ship position
        if (EnemyShipPos && Game.EnemyShip != null)
            EnemyShipPos.position = Game.EnemyShip.GetPosition();


        //Set Bullets position
        for (int i = 0; i < BulletsPos.Count; i++)
        {
            BulletsPos[i].position = Game.Bullets[i].GetPosition();
            BulletsPos[i].rotation = Quaternion.Euler(0, 0, Game.Bullets[i].GetRotation());

            //NEXT UPDATE SPOILERS
            //if (Game.Bullets[i].MyBullet)
            //    BulletsPos[i].GetComponent<Image>().color = Color.white;
            //else
            //    BulletsPos[i].GetComponent<Image>().color = Color.red;
        }

        //Set Small Asteroids position
        for (int i = 0; i < SmallAsteroidsPos.Count; i++)
        {
            SmallAsteroidsPos[i].position = Game.SmallAsteroids[i].GetPosition();
            SmallAsteroidsPos[i].rotation = Quaternion.Euler(0, 0, Game.SmallAsteroids[i].GetRotation());
        }

        //Set Big Asteroids position
        for (int i = 0; i < BigAsteroidsPos.Count; i++)
        {
            BigAsteroidsPos[i].position = Game.BigAsteroids[i].GetPosition();
            BigAsteroidsPos[i].rotation = Quaternion.Euler(0, 0, Game.BigAsteroids[i].GetRotation());
        }
    }

    //-------------------------------
    // Invokes
    //-------------------------------
    private void Game_OnNewLaser()
    {
        GameObject NewLaser = Instantiate(LaserPref, transform);
        NewLaser.transform.position = PlayerShip.position;
        NewLaser.transform.rotation = PlayerShip.rotation;
    }

    private void Game_OnGameUi(int score, Vector2 pos, float rot, float spd, int health, int lasershots, float laserDelay, bool GameActive)
    {
        Position.text = $"Pos: {pos.x.ToString("0000")} : {pos.y.ToString("0000")}  Rot: {Mathf.Round(rot)}  Spd: {Mathf.Round(spd)}";
        Score.text = $"SCORE: {score}\nHEALTH: {health}\nLASERS: {lasershots}\nLASER RELOAD: {Mathf.Round(laserDelay)}";

        if (!GameActive)
        {
            EndScreen.SetActive(true);
            FinalScore.gameObject.SetActive(true);
            FinalScore.text = $"YOUR SCORE: {score}";
        }

    }

    private void Game_OnNewEnemy()
    {
        Debug.Log("New Enemy Ship");

        EnemyShipPos = Instantiate(EnemyShip, transform).transform;
    }
    private void Game_OnDeadEnemy()
    {
        Debug.Log("Destroy Enemy Ship");

        Destroy(EnemyShipPos.gameObject);
    }

    private void Game_OnNewBullet()
    {
        Debug.Log("New Bullet");
        GameObject NewBullet;
        NewBullet = Instantiate(BulletPref, transform);
        BulletsPos.Add(NewBullet.transform);
    }
    private void Game_OnDeadBullet(int id)
    {
        Destroy(BulletsPos[id].gameObject);
        BulletsPos.RemoveAt(id);
    }

    private void Game_OnNewAsteroid(bool Big)
    {
        if (Big)
        {
            Debug.Log("New Big Asteroid");
            GameObject NewAsteroid = Instantiate(BigAsteroidPref, transform);
            BigAsteroidsPos.Add(NewAsteroid.transform);
        }
        else
        {
            Debug.Log("New Small Asteroid");
            GameObject NewAsteroid = Instantiate(SmallAsteroidPref, transform);
            SmallAsteroidsPos.Add(NewAsteroid.transform);
        }
    }
    private void Game_OnDeadAsteroid(bool Big, int id)
    {
        if (Big)
        {
            Debug.Log("Destroyed Big Asteroid");
            Destroy(BigAsteroidsPos[id].gameObject);
            BigAsteroidsPos.RemoveAt(id);
        }
        else
        {
            Debug.Log("Destroyed Small Asteroid");
            Destroy(SmallAsteroidsPos[id].gameObject);
            SmallAsteroidsPos.RemoveAt(id);
        }
    }
}
