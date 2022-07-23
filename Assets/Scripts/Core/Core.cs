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
    public List<ObjectTransform> SmallAsteroids;
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
    public delegate void DeadAsteroid(bool Big, int i);
    public event NewAsteroid OnNewAsteroid;
    public event DeadAsteroid OnDeadAsteroid;

    //like Start()
    public void GameStart()
    {
        Active = true;
        PlayerShip = new Player(new Vector2(500,500), 0);
        SmallAsteroids = new List<ObjectTransform>();
        BigAsteroids = new List<ObjectTransform>();
        Bullets = new List<Bullet>(); 
        
        SpawnNewAsteroid();
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
            SpawnNewAsteroid();
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
    void SpawnNewAsteroid()
    {
        if (Random.Range(0, 100) < 50)
        {
            SpawnNewSmallAsteroid();
        }
        else
        {
            SpawnNewBigAsteroid();
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

        for (int i = 0; i < BigAsteroids.Count; i++)
        {
            Debug.Log(FindNearestPointOnLine(PlayerShip.GetPosition(), PlayerShip.GetPosition() + PlayerShip.GetDirection() * 1000, BigAsteroids[i].GetPosition()));
            if (FindNearestPointOnLine(PlayerShip.GetPosition(), PlayerShip.GetPosition() + PlayerShip.GetDirection() * 1000, BigAsteroids[i].GetPosition()) < LaserAngle)
            {
                DestroyBigAsteroid(i);
                i--;
            }
        }
        for (int i = 0; i < SmallAsteroids.Count; i++)
        {
            Debug.Log(FindNearestPointOnLine(PlayerShip.GetPosition(), PlayerShip.GetPosition() + PlayerShip.GetDirection() * 1000, SmallAsteroids[i].GetPosition()));
            if (FindNearestPointOnLine(PlayerShip.GetPosition(), PlayerShip.GetPosition() + PlayerShip.GetDirection() * 1000, SmallAsteroids[i].GetPosition()) < LaserAngle)
            {
                DestroySmallAsteroid(i);
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
    void SpawnNewSmallAsteroid()
    {
        ObjectTransform newAsteroid = new ObjectTransform(new Vector2(
            Random.Range(-20, 1020), 1000),
            Random.Range(0, 360), 
            Random.Range(30, 50)
            );
        SmallAsteroids.Add(newAsteroid);
        OnNewAsteroid?.Invoke(false);
    }
    void SpawnNewSmallAsteroid(Vector2 pos)
    {
        ObjectTransform newAsteroid = new ObjectTransform(
            pos, 
            Random.Range(0, 360), 
            Random.Range(30, 50)
            );
        SmallAsteroids.Add(newAsteroid);
        OnNewAsteroid?.Invoke(false);
    }
    void SpawnNewBigAsteroid()
    {
        ObjectTransform newAsteroid = new ObjectTransform(
            new Vector2(Random.Range(-20, 1020), 1000), 
            Random.Range(0, 360), 
            Random.Range(10, 30)
            );
        BigAsteroids.Add(newAsteroid);
        OnNewAsteroid?.Invoke(true);
    }

    //destroy asteroid with i Id
    void DestroySmallAsteroid(int i)
    {
        Score += 10;
        SmallAsteroids.RemoveAt(i);
        OnDeadAsteroid?.Invoke(false, i);
    }
    void DestroyBigAsteroid(int i)
    {
        Score += 5;
        for (int c = 0; c < Random.Range(2, 5); c++)
        {
            SpawnNewSmallAsteroid(BigAsteroids[i].GetPosition());
        }

        BigAsteroids.RemoveAt(i);
        OnDeadAsteroid?.Invoke(true, i);
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

            //Check distances to Small Asteroids
            for (int a = 0; a < SmallAsteroids.Count; a++)
            {
                if(Vector3.Distance(Bullets[i].GetPosition(), SmallAsteroids[a].GetPosition()) < 20)
                {
                    Bullets.RemoveAt(i);
                    OnDeadBullet?.Invoke(i);
                    i--;
                    DestroySmallAsteroid(a);
                    break;
                }
            }

            if (i < 0) continue;

            //Check distances to Big Asteroids
            for (int a = 0; a < BigAsteroids.Count; a++)
            {
                if(Vector3.Distance(Bullets[i].GetPosition(), BigAsteroids[a].GetPosition()) < 30)
                {
                    Bullets.RemoveAt(i);
                    OnDeadBullet?.Invoke(i);
                    i--;
                    DestroyBigAsteroid(a);
                    break;
                }
            }

        }
    }
    void UpdateAsteroids()
    {
        //Update position foreach Small Asteroid and check distances to Player ship
        for (int i = 0; i < SmallAsteroids.Count; i++)
        {
            SmallAsteroids[i].UpdatePos(); 
            if (Vector3.Distance(PlayerShip.GetPosition(), SmallAsteroids[i].GetPosition()) < 30)
            {
                SmallAsteroids.RemoveAt(i);
                OnDeadAsteroid?.Invoke(false, i);

                if (PlayerShip.DeadCrash())
                    GameEnd();
                break;
            }
        }
        //Update position foreach Big Asteroid and check distances to Player ship
        for (int i = 0; i < BigAsteroids.Count; i++)
        {
            BigAsteroids[i].UpdatePos();
            if (Vector3.Distance(PlayerShip.GetPosition(), BigAsteroids[i].GetPosition()) < 50)
            {
                BigAsteroids.RemoveAt(i);
                OnDeadAsteroid?.Invoke(true, i);

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
