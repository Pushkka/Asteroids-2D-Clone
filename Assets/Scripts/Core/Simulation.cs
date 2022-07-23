using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Simulation
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
    public Player PlayerShip = new Player(new Vector2(500,500), 0);
    public Enemy EnemyShip;
    public List<Bullet> Bullets;
    public List<Asteroid> Asteroids;

    //-------------------------------
    // Invokes for UI updates
    //-------------------------------
    public delegate void GameUi(int score, Vector2 pos, float rot, float spd, int health, int lasershots, float laserDelay, bool active);
    public event GameUi OnGameUi;

    public delegate void NewEnemy();
    public event NewEnemy OnNewEnemy;
    public event NewEnemy OnDeadEnemy;

    public delegate void DeadBullet(int i);
    public event DeadBullet OnDeadBullet;

    public delegate void NewAsteroid(bool Big);
    public delegate void DeadAsteroid(int i);
    public event NewAsteroid OnNewAsteroid;
    public event DeadAsteroid OnDeadAsteroid;

    public void GameStart()
    {
        Active = true;
        PlayerShip = new Player(new Vector2(500,500), 0);
        Asteroids = new List<Asteroid>();
        Bullets = new List<Bullet>();

        PlayerShip.OnShot += SpawnNewBullet;
        PlayerShip.OnLaserShot += SpawnLaser;
    }
    public void GameTick(float DeltaTime)
    {
        if (!Active)
            return;

        //Увеличивает время уровня
        LevelTime += DeltaTime;

        //Простенькие таймеры на спавн Астероидов и летающих тарелок
        if (LevelTime % 2 <= 0.02f)
            SpawnAsteroids();
        if (LevelTime % 10 <= 0.02f)
            SpawnNewEnemyShip();

        PlayerShip.Update();
        UpdateBullets();
        UpdateAsteroids();
        UpdateEnemyShip();

        //Инвок для интерфейса
        OnGameUi?.Invoke(Score, PlayerShip.GetPosition(), PlayerShip.GetRotation(), PlayerShip.GetVelosity().magnitude, PlayerShip.Health, PlayerShip.LaserCount, PlayerShip.LaserReload, Active);
    }
    public void GameEnd()
    {
        Active = false;
    }


    //-------------------------------
    // Функции для спавна противкников
    //-------------------------------
    private void SpawnAsteroids()
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
    private void SpawnNewEnemyShip()
    {
        if (EnemyShip != null)
            return;

        EnemyShip = new Enemy( new Vector2(-20, Random.Range(-20, 1020)) );
        OnNewEnemy?.Invoke();
    }


    //-------------------------------
    // Функции уничтожения противников
    //-------------------------------
    private void DestroyAsteroid(int i)
    {
        Score += 10;
        Asteroids.RemoveAt(i);
        OnDeadAsteroid?.Invoke(i);
    }
    private void DestroyEnemyShip()
    {
        Score += 25;
        EnemyShip = null;
        OnDeadEnemy?.Invoke();
    }



    //-------------------------------
    // Функции для выстрелок игрока
    //-------------------------------
    private float FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
    { 
        //математическая функция для нахождения минимальной дистанции до прямой линии

        Vector3 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        Vector3 lhs = point - origin;
        float dotP = Vector3.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return Vector3.Distance(point, origin + heading * dotP);
    }
    private void SpawnLaser()
    {
        float LaserAngle = 80;

        if (EnemyShip != null && 
            FindNearestPointOnLine(PlayerShip.GetPosition(), PlayerShip.GetPosition() + PlayerShip.GetDirection() * 1000, EnemyShip.GetPosition()) < LaserAngle)
        {
                EnemyShip = null;
            OnDeadEnemy?.Invoke();
        }

        for (int i = 0; i < Asteroids.Count; i++)
        {
            if (FindNearestPointOnLine(PlayerShip.GetPosition(), PlayerShip.GetPosition() + PlayerShip.GetDirection() * 1000, Asteroids[i].GetPosition()) < LaserAngle)
            {
                DestroyAsteroid(i);
                i--;
            }
        }
    }
    private void SpawnNewBullet()
    {
        if (Bullets.Count >= 5)
        {
            Bullets.RemoveAt(0);
            OnDeadBullet?.Invoke(0);
        }

        Bullet NewBullet = new Bullet(PlayerShip.GetPosition(), PlayerShip.GetRotation(), 200, true);
        Bullets.Add(NewBullet);
    }


    //-------------------------------
    // Подфункции для спавка астероидов
    //-------------------------------
    private void SpawnNewAsteroid(bool Big)
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
    private void SpawnNewAsteroid(bool Big, Vector2 pos)
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



    //-------------------------------
    // Функции для обновления обьектов симуляции
    //-------------------------------
    private void UpdateBullets()
    {
        //Обновляет позицию каждой пули, и проверяет дистанцию до каждого астероида
        for (int i = 0; i < Bullets.Count; i++)
        {
            Bullets[i].UpdatePosition();

            //Проверяет дистанцию до фражесского корабля, Если он есть
            if (EnemyShip != null && Vector3.Distance(Bullets[i].GetPosition(), EnemyShip.GetPosition()) < 15)
            {
                Bullets.RemoveAt(i);
                OnDeadBullet?.Invoke(i);
                i--; 
                DestroyEnemyShip();
                continue;
            }

            //Проверяет дистанцию до каждого астероида
            for (int a = 0; a < Asteroids.Count; a++)
            {
                float range = Asteroids[a].BigAsteroid ? 20 : 40;
                if (Vector3.Distance(Bullets[i].GetPosition(), Asteroids[a].GetPosition()) < range)
                {
                    //Если астероид большой, он раскалывается 
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
    private void UpdateAsteroids()
    {
        //Обновляет позицию каждого астероида, и проверяет дистанцию до корабля игрока
        for (int i = 0; i < Asteroids.Count; i++)
        {
            Asteroids[i].UpdatePosition();

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
    private void UpdateEnemyShip()
    {
        if (EnemyShip != null)
        {
            EnemyShip.MoveTo(PlayerShip.GetPosition());
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
