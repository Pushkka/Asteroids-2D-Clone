using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core
{
    //-------------------------------
    // Game Stats
    //-------------------------------
    public float LevelTime;
    public int Score;
    public bool Active;

    //-------------------------------
    // Game Objects
    //-------------------------------
    public Player PlayerShip;
    public ObjectTransform EnemyShip;
    public List<Bullet> Bullets;
    public List<Asteroid> Asteroids;
    public List<ObjectTransform> BigAsteroids;

    //-------------------------------
    // Invokes for UI updates
    //-------------------------------
    public delegate void GameUi(int score, Vector2 pos, float rot, float spd, int health, int lasershots, float laserDelay, bool active);
    public event GameUi OnGameUi;

    public delegate void NewEnemy();
    public event NewEnemy OnNewEnemy;
    public event NewEnemy OnDeadEnemy;

    public delegate void NewBullet();
    public delegate void DeadBullet(int i);
    public event NewBullet OnNewLaser;
    public event NewBullet OnNewBullet;
    public event DeadBullet OnDeadBullet;

    public delegate void NewAsteroid(bool Big);
    public delegate void DeadAsteroid(int i);
    public event NewAsteroid OnNewAsteroid;
    public event DeadAsteroid OnDeadAsteroid;

    //like Start()
    public void GameStart()
    {
        Active = true;
        PlayerShip = new Player(new Vector2(500,500), 0);
        Asteroids = new List<Asteroid>();
        BigAsteroids = new List<ObjectTransform>();
        Bullets = new List<Bullet>(); 
        
        SpawnAsteroids();
    }

    //Game tick, like Update()
    public void GameTick(float DeltaTime)
    {
        if (!Active)
            return;

        //Increace Level Time
        LevelTime += DeltaTime;

        //Simple timers
        if (LevelTime % 2 <= 0.02f)
            SpawnAsteroids();
        if (LevelTime % 10 <= 0.02f)
            SpawnNewEnemyShip();

        //subUpdates for some objects
        UpdatePlayer(DeltaTime);
        UpdateBullets();
        UpdateAsteroids();
        UpdateEnemyShip(DeltaTime);

        //Game UI Invoke
        OnGameUi?.Invoke(Score, PlayerShip.GetPosition(), PlayerShip.GetRotation(), PlayerShip.Move.magnitude, PlayerShip.Health, PlayerShip.LaserCount, PlayerShip.LaserReload, Active);
    }
    public void GameEnd()
    {
        Active = false;
    }

    //main spawn new asteroids Functions
    void SpawnAsteroids()
    {
        if (Random.Range(0, 100) < 50)
        {
            SpawnNewAsteroid(false);
        }
        else
        {
            SpawnNewAsteroid(true);
        }
    }
    //main spawn EnemyShip Functions
    void SpawnNewEnemyShip()
    {
        if (EnemyShip != null)
            return;

        EnemyShip = new ObjectTransform(new Vector2(
            -20, Random.Range(-20, 1020)),
            90
            );
        OnNewEnemy?.Invoke();
    }
    //main destroy EnemyShip Functions
    void DestroyEnemyShip()
    {
        Score += 25;
        EnemyShip = null;
        OnDeadEnemy?.Invoke();
    }


    //математическая функция для нахождения минимальной дистанции до прямой линии
    float FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
    {
        //Get heading
        Vector3 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector3 lhs = point - origin;
        float dotP = Vector3.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return Vector3.Distance(point, origin + heading * dotP);
    }
    void SpawnLaser()
    {
        OnNewLaser?.Invoke();
        float LaserAngle = 80;

        if (EnemyShip != null && 
            FindNearestPointOnLine(PlayerShip.GetPosition(), PlayerShip.GetPosition() + PlayerShip.GetDirection() * 1000, EnemyShip.GetPosition()) < LaserAngle)
        {
                EnemyShip = null;
            OnDeadEnemy?.Invoke();
        }

        for (int i = 0; i < Asteroids.Count; i++)
        {
            Debug.Log(FindNearestPointOnLine(PlayerShip.GetPosition(), PlayerShip.GetPosition() + PlayerShip.GetDirection() * 1000, Asteroids[i].GetPosition()));
            if (FindNearestPointOnLine(PlayerShip.GetPosition(), PlayerShip.GetPosition() + PlayerShip.GetDirection() * 1000, Asteroids[i].GetPosition()) < LaserAngle)
            {
                DestroyAsteroid(i);
                i--;
            }
        }
    }
    void SpawnNewBullet()
    {
        if (Bullets.Count >= 5)
        {
            Bullets.RemoveAt(0);
            OnDeadBullet?.Invoke(0);
        }

        Bullet NewBullet = new Bullet(PlayerShip.GetPosition(), PlayerShip.GetRotation(), 200, true);
        Bullets.Add(NewBullet);

        OnNewBullet?.Invoke();
    }


    //spawn new asteroids subFunctions
    void SpawnNewAsteroid(bool Big)
    {
        float spd = Big ? Random.Range(10, 30) : Random.Range(30, 50);
        Asteroid newAsteroid = new Asteroid(new Vector2(
            Random.Range(-20, 1020), 1000),
            Random.Range(0, 360),
            spd,
            Big
            );
        Asteroids.Add(newAsteroid);
        OnNewAsteroid?.Invoke(Big);
    }
    void SpawnNewAsteroid(bool Big, Vector2 pos)
    {
        float spd = Big ? Random.Range(10, 30) : Random.Range(30, 50);
        Asteroid newAsteroid = new Asteroid(
            pos, 
            Random.Range(0, 360),
            spd,
            Big
            );
        Asteroids.Add(newAsteroid);
        OnNewAsteroid?.Invoke(Big);
    }

    //destroy asteroid with i Id
    void DestroyAsteroid(int i)
    {
        Score += 10;
        OnDeadAsteroid?.Invoke(i);
        Asteroids.RemoveAt(i);
    }


    //subUpdates for objects
    void UpdatePlayer(float DeltaTime)
    {
        //Check input keybd buttons
        Vector2 Input = PlayerInput.GetMovementInput();
        PlayerShip.AddSpeed(Input.x);
        PlayerShip.AddRotation(Input.y * 3);
        PlayerShip.UpdatePos();

        //Decreace Ship Reload Timers
        PlayerShip.DecreaceDelay(DeltaTime);

        if (PlayerInput.IsShoot() && PlayerShip.ShotDelay <= 0)
        {
            PlayerShip.Shot();
            SpawnNewBullet();
        }
        if (PlayerInput.IsLaserShoot() && PlayerShip.LaserDelay <= 0)
        {
            PlayerShip.ShotLaser();
            SpawnLaser();
        }
    }
    void UpdateBullets()
    {
        //Update position foreach Bullet and check distances to Objects
        for (int i = 0; i < Bullets.Count; i++)
        {
            //Update position
            Bullets[i].UpdatePos();

            //Check distances to Enemy ship
            if (EnemyShip != null && Vector3.Distance(Bullets[i].GetPosition(), EnemyShip.GetPosition()) < 15)
            {
                Bullets.RemoveAt(i);
                OnDeadBullet?.Invoke(i);
                i--; 
                DestroyEnemyShip();
                continue;
            }

            //Check distances to Asteroids
            for (int a = 0; a < Asteroids.Count; a++)
            {
                float range = Asteroids[a].BigAsteroid ? 20 : 40;
                if (Vector3.Distance(Bullets[i].GetPosition(), Asteroids[a].GetPosition()) < range)
                {
                    Debug.Log(Asteroids[a].BigAsteroid);
                    if (Asteroids[a].BigAsteroid)
                    {
                        SpawnNewAsteroid(false, Asteroids[a].GetPosition());
                        SpawnNewAsteroid(false, Asteroids[a].GetPosition());
                        SpawnNewAsteroid(false, Asteroids[a].GetPosition());
                    }

                    Bullets.RemoveAt(i);
                    OnDeadBullet?.Invoke(i);
                    i--;
                    DestroyAsteroid(a);
                    break;
                }
            }
        }
    }
    void UpdateAsteroids()
    {
        //Update position foreach Small Asteroid and check distances to Player ship
        for (int i = 0; i < Asteroids.Count; i++)
        {
            Asteroids[i].UpdatePos();
            float range = Asteroids[i].BigAsteroid ? 20 : 40;
            if (Vector3.Distance(PlayerShip.GetPosition(), Asteroids[i].GetPosition()) < range)
            {
                Asteroids.RemoveAt(i);
                OnDeadAsteroid?.Invoke(i);

                if (PlayerShip.DeadCrash())
                    GameEnd();
                break;
            }
        }
    }
    void UpdateEnemyShip(float TimeSpent)
    {
        if (EnemyShip != null)
        {
            //Move Emeny ship in to Player ship
            EnemyShip.Move = Vector3.Lerp(EnemyShip.Move, (PlayerShip.GetPosition() - EnemyShip.GetPosition()).normalized * 80, TimeSpent);
            EnemyShip.UpdatePos();

            //Check Distance
            if(Vector3.Distance(PlayerShip.GetPosition(), EnemyShip.GetPosition()) < 12)
            {
                EnemyShip = null;
                OnDeadEnemy?.Invoke();

                if (PlayerShip.DeadCrash())
                    GameEnd();
            }
        }
    }
}
