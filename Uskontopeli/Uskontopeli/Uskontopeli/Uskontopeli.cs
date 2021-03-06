﻿using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class Uskontopeli : PhysicsGame
{
    PhysicsObject player;
    PhysicsObject Goal;
    PhysicsObject ammus;
    PhysicsObject Enemy1;
    PhysicsObject Fact1;
    PhysicsObject health;

    GameObject fakta;

    AssaultRifle playerWeapon1;

    private Image[] playerWalkDown = LoadImages("PappiAnimA1", "PappiAnimA2", "PappiAnimA1","PappiAnimA3");
    private Image[] playerWalkUp = LoadImages("PappiAnimY1", "PappiAnimY2");
    private Image[] playerWalkLeft = LoadImages("PappiAnimV1", "PappiAnimV2", "PappiAnimV1", "PappiAnimV3");
    private Image[] playerWalkRight = LoadImages("PappiAnimO1", "PappiAnimO2", "PappiAnimO1", "PappiAnimO3");
    private Image[] playerIdle = LoadImages("PappiAnimA1", "Pappi2");
    private Image[] Enemy1Idle = LoadImages("Vihollinen1", "Viholline2");

    Image projectile1 = LoadImage("Projectile1");
    Image Pappikuva = LoadImage("PappiAnimA1");
    Image Fact1Image = LoadImage("Fakta1");
    Image Fact2Image = LoadImage("Fakta2");
    Image Fact3Image = LoadImage("Fakta3");
    Image Fact4Image = LoadImage("Fakta4");
    Image Fact5Image = LoadImage("Fakta5");
    Image Fact8Image = LoadImage("Fakta8");
    Image Seinakuva = LoadImage("seinä");
    Image taustaKuva = LoadImage("Hiekka");

    [Save] public int KenttaNumero = 1;
    [Save] public int FactNro = 0;

    [Save] public DoubleMeter PlayerLife;

    public override void Begin()
    {
        ClearAll();
        Level.Background.Color = Color.Black;
        MainMenu();
    }

    void MainMenu()
    {
        MultiSelectWindow mainMenu = new MultiSelectWindow("Tervetuloa", "Pelaa", "Poistu pelistä" );
        mainMenu.AddItemHandler(0, NewGame);
        mainMenu.AddItemHandler(1, Exit);

        Add(mainMenu);
    }

    void StartGame()
    {
        if (DataStorage.Exists("tilanne"))
        {

            LoadGame("tilanne");
            NextLevel();
            MessageDisplay.Add("Jatka peliä painamalla 'P'");

        }
        else
        {
            NewGame();
        }

    }

    void NewGame()
    {
        NextLevel();

    }

    void NextLevel()
    {
        ClearAll();
        Level.Background.Color = Color.Wheat;
        //Level.Background.Image = taustaKuva;
        //Level.Background.TileToLevel();

        if (KenttaNumero == 1) Luokentta("Kartta1");
        else if (KenttaNumero == 2) Luokentta("mapui2");
        else if (KenttaNumero == 3) Luokentta("Kartta3");
        else if (KenttaNumero == 4) Luokentta("Kartta4");
        else if (KenttaNumero == 5) Exit();
    }

    void Luokentta(string KenttanNimi)
    {

        ColorTileMap kentta = ColorTileMap.FromLevelAsset(KenttanNimi);
        kentta.SetTileMethod("#FF000000", AddWall);
        kentta.SetTileMethod("#FFFF000C", AddGoal);
        kentta.SetTileMethod("#FFFAFF07", AddFact1);
        kentta.SetTileMethod("#FFFAFF08", AddAbility1);
        kentta.SetTileMethod("#FF0015FF", CreateEnemy1);
        kentta.SetTileMethod("#FFFF00B2", Addplayer);
        kentta.SetTileMethod("#FF13FF08", AddHealthBox);

        kentta.Execute(100, 100);
        Camera.Follow(player);
        AddControlls();
        IsFullScreen = false;
        IsMouseVisible = true;
        Camera.Zoom(2);

    }
    
    void AddWall(Vector paikka, double korkeus, double leveys)
    {
        PhysicsObject Wall = new PhysicsObject(100, 100);
        Wall.Color = Color.Black;
        Wall.CollisionIgnoreGroup = 2;
        Wall.Position = paikka;
        Wall.Image = Seinakuva;
        Wall.MakeStatic();
        Wall.CanRotate = false;
        Add(Wall);
    }

    void Addplayer(Vector paikka, double korkeus, double leveys)
    {
        player = new PhysicsObject(75, 100);
        player.Restitution = 0;
        player.Image = Pappikuva;
        player.Position = paikka;
        player.CanRotate = false;
        player.MaxVelocity = 300;
        player.IgnoresExplosions = true;
        player.Tag = "Pelaaja";
        Add(player);

        playerWeapon1 = new AssaultRifle(30, 10);
        playerWeapon1.ProjectileCollision = Hit;
        playerWeapon1.InfiniteAmmo = true;
        playerWeapon1.Power.Value = 2000;
        playerWeapon1.IsVisible = false;
        playerWeapon1.AttackSound = null;
        player.Add(playerWeapon1);

        IdlePlayer();

        PlayerLife = new DoubleMeter(100);
        PlayerLife.MaxValue = 100;
        BarGauge PlayerLifeBar = new BarGauge(20, Screen.Width / 3);
        PlayerLifeBar.X = Screen.Left + Screen.Width / 2;
        PlayerLifeBar.Y = Screen.Top - 40;
        PlayerLifeBar.Angle = Angle.FromDegrees(90);
        PlayerLifeBar.BindTo(PlayerLife);
        PlayerLifeBar.Color = Color.Red;
        PlayerLifeBar.BarColor = Color.Green;
        Add(PlayerLifeBar);

    }

    void AddControlls()
    {
        Keyboard.Listen(Key.S, ButtonState.Pressed, delegate
        {
            player.Animation = new Animation(playerWalkDown);
            player.Animation.FPS = 10;
            player.Animation.Start();

        }, null);
        Keyboard.Listen(Key.S, ButtonState.Down, delegate
        {
            MovePlayer(new Vector(0, -500));

        }, null);
        Keyboard.Listen(Key.S, ButtonState.Released, delegate
        {
            IdlePlayer();
            player.Stop();
        }, null );

        Keyboard.Listen(Key.D, ButtonState.Pressed, delegate
        {
            player.Animation = new Animation(playerWalkRight);
            player.Animation.FPS = 10;
            player.Animation.Start();

        }, null);
        Keyboard.Listen(Key.D, ButtonState.Down, delegate
        {
            MovePlayer(new Vector(500, 0));

        }, null);
        Keyboard.Listen(Key.D, ButtonState.Released, delegate
        {
            IdlePlayer();
            player.Stop();
        }, null);

        Keyboard.Listen(Key.W, ButtonState.Pressed, delegate
        {
            player.Animation = new Animation(playerWalkUp);
            player.Animation.FPS = 5;
            player.Animation.Start();

        }, null);
        Keyboard.Listen(Key.W, ButtonState.Down, delegate
        {
            MovePlayer(new Vector(0, 500));

        }, null);
        Keyboard.Listen(Key.W, ButtonState.Released, delegate
        {
            IdlePlayer();
            player.Stop();
        }, null);

        Keyboard.Listen(Key.A, ButtonState.Pressed, delegate
        {
            player.Animation = new Animation(playerWalkLeft);
            player.Animation.FPS = 10;
            player.Animation.Start();

        }, null);
        Keyboard.Listen(Key.A, ButtonState.Down, delegate
        {
            MovePlayer(new Vector(-500, 0));

        }, null);
        Keyboard.Listen(Key.A, ButtonState.Released, delegate
        {
            IdlePlayer();
            player.Stop();
        }, null);

        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, Shoot, "shoot", playerWeapon1);
        Mouse.ListenMovement(0.1, Aim, "Tähtää aseella");

        Keyboard.Listen(Key.F1, ButtonState.Pressed, delegate
        {
            KenttaNumero++;
            NextLevel();
        }, "Skippaa taso");

        Keyboard.Listen(Key.F2, ButtonState.Pressed, delegate
        {
            FactNro++;
        }, "Löydä fakta");

        Keyboard.Listen(Key.F3, ButtonState.Pressed, delegate
        {
            PlayerLife.Value -= 10;

            if (PlayerLife == 0)
            {
                Dead();
            }
        }, "Vähennä elämää");


        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed,delegate
        {
            Pause();
            PauseMenu();
        },null);

        Keyboard.Listen(Key.Enter, ButtonState.Pressed, delegate
        {
            if (fakta == null)
            {
                return;
            }
            else
            {
                fakta.Destroy();
            }
        }
, null);
        Keyboard.Listen(Key.F9, ButtonState.Pressed, delegate
        {
            MessageDisplay.Add("factnum: " + FactNro);
        }, null
        );

        Keyboard.Listen(Key.P, ButtonState.Pressed, Pause, null);

    }

    void AddGoal(Vector paikka, double korkeus, double leveys)
    {
        Goal = new PhysicsObject(100, 100);
        Goal.Position = paikka;
        Goal.IsVisible = true;
        Goal.Color = Color.Red;
        Goal.IgnoresPhysicsLogics= true;
        Add(Goal);

        AddCollisionHandler(player, Goal,SeuraavaKentta);
    }

    void MovePlayer(Vector Vektori )
    {
        player.Push(Vektori);
    }

    void IdlePlayer()
    {
        player.Animation = new Animation(playerIdle);
        player.Animation.FPS = 2;
        player.Animation.Start();
    }

    void EnemyHit(PhysicsObject tormaaja, PhysicsObject target)
    {
        PlayerLife.Value -= 10;
        Explosion rajahdys = new Explosion(100);
        rajahdys.Position = player.Position;
        rajahdys.Sound = null;
        rajahdys.IsVisible = false;
        rajahdys.ShockwaveColor = Color.Transparent;
        rajahdys.Speed = 500;
        rajahdys.Force = 100;

        Add(rajahdys);

        if (PlayerLife == 0)
        {
            Dead();
        }
    }

    void Hit(PhysicsObject ammus, PhysicsObject target)
    {
            ammus.Destroy();

        if (target.Tag == "Vihu")
        {
            target.Destroy();
        }
    }

    void Shoot(AssaultRifle weapon)
    {
        ammus = playerWeapon1.Shoot();
        if(ammus != null)
        {
            //ammus.RotateImage = false;
            ammus.Size *= 3;
            ammus.Image = projectile1;
            ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
            ammus.Tag = "Projektile";
            
        }

    }

    void Aim (AnalogState hiirenLiike)
    {
        if (playerWeapon1 == null) return;
      Vector suunta = (Mouse.PositionOnWorld - playerWeapon1.AbsolutePosition).Normalize();
      playerWeapon1.Angle = suunta.Angle;
    
    }

    void AddFact1(Vector paikka, double korkeus, double leveys)
    {
        Fact1 = new PhysicsObject(100, 100);
        Fact1.Color = Color.Yellow;
        Fact1.Position = paikka;
        Fact1.IgnoresPhysicsLogics = true;
        Enemy1.IgnoresCollisionWith(Enemy1);
        Fact1.MakeStatic();
        Add(Fact1);

        AddCollisionHandler(player, Fact1, ShowAFact);
    }

    void CreateEnemy1 (Vector paikka, double korkeus, double leveys)
    {
        Enemy1 = new PhysicsObject(75, 100);
        Enemy1.Color = Color.HotPink;
        Enemy1.Shape = Shape.Rectangle;
        Enemy1.Position = paikka;
        Enemy1.CanRotate = false;
        Enemy1.Tag = "Vihu";
        Enemy1.Animation = new Animation(Enemy1Idle);
        Enemy1.Animation.FPS = 2;
        Enemy1.Animation.Start();
        Enemy1.Restitution = 1;
        Add(Enemy1);
        

        FollowerBrain Enemy1brain = new FollowerBrain("Pelaaja");
        Enemy1brain.Speed = 350;
        Enemy1brain.DistanceFar = 500;
        Enemy1.Brain = Enemy1brain;
        Enemy1brain.Active = true;

        
        AddCollisionHandler(Enemy1, player, EnemyHit);
    }

    void SeuraavaKentta(PhysicsObject tormaaja, PhysicsObject kohde)
    {

        SaveGame("tilanne.xml");
        KenttaNumero++;
        ClearAll();
        Level.Background.Color = Color.Black;
        MessageDisplay.Add("Siirrytään seuraavaan tasoon...");

        Timer NextLevelTimer = new Timer();
        NextLevelTimer.Interval = 1;
        NextLevelTimer.Timeout += delegate
        {
            NextLevel();
        };
        NextLevelTimer.Start(1);

    }

    void AddAbility1(Vector paikka, double korkeus, double leveys)
    {
        PhysicsObject PickA1 = new PhysicsObject(100, 100);
        PickA1.Color = Color.Green;
        PickA1.Position = paikka;
        PickA1.MakeStatic();
        Add(PickA1);
    }

    void AddHealthBox(Vector paikka, double korkeus, double leveys)
    {
        health = new PhysicsObject(100, 100);
        health.Position = paikka;
        health.Color = Color.Green;
        health.MakeStatic();
        health.IsVisible = true;
        Add(health);

        AddCollisionHandler(player, health, AddHealth);
    }

    void ShowAFact(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        FactNro++;

        GameObject fakta = new GameObject(300, 500);
        fakta.Position = player.Position;
        Add(fakta);

        if (FactNro == 1)
        {
            fakta.Image = Fact1Image;
        }

        else if (FactNro == 2)
        {
            fakta.Image = Fact2Image;
        }

        else if (FactNro == 3)
        {
            fakta.Image = Fact3Image;
        }

        else if (FactNro == 4)
        {
            fakta.Image = Fact4Image;
        }

        else if (FactNro == 5)
        {
            fakta.Image = Fact5Image;
        }

        else if (FactNro == 6)
        {
            fakta.Image = Fact8Image;
        }

        else
        {
            MessageDisplay.Add("Olet jo kerännyt kaikki faktat!");
        }

        kohde.Destroy();

        Timer.SingleShot(10, fakta.Destroy);
    }

     void PauseMenu()
    {
        ClearControls();
        AddControlls();

        MultiSelectWindow PauseValikko = new MultiSelectWindow("Peli on pysäytty", "Jatka peliä", "Kerätyt faktat", "Päävalikko");
        PauseValikko.AddItemHandler(0, Pause);
        PauseValikko.AddItemHandler(1, FactMenu);
        PauseValikko.AddItemHandler(2, Begin);
        Add(PauseValikko);
    }

    void FactMenu()
    {
        ClearControls();
        if (FactNro == 0)
        {
            MultiSelectWindow FaktaMenu = new MultiSelectWindow("Et ole löytänyt yhtään faktaa vielä", "Jatka peliä");
            FaktaMenu.AddItemHandler(0, delegate
            {
                FaktaMenu.Destroy();
                Pause();
            });
            Add(FaktaMenu);
        }
       else if (FactNro == 1)
        {
            MultiSelectWindow FaktaMenu = new MultiSelectWindow("Kerätyt faktat", "fakta 1", "takaisin");
            FaktaMenu.AddItemHandler(0, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact1Image);
            } );
            FaktaMenu.AddItemHandler(1, delegate
            {
                PauseMenu();
                FaktaMenu.Destroy();
            });
            Add(FaktaMenu);
        }

       else if (FactNro == 2)
        { 
        MultiSelectWindow FaktaMenu = new MultiSelectWindow("Kerätyt faktat", "fakta 1", "fakta 2", "takaisin");
            FaktaMenu.AddItemHandler(0, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact1Image);
            });
            FaktaMenu.AddItemHandler(1, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact2Image);
            });
            FaktaMenu.AddItemHandler(2, delegate
            {
                PauseMenu();
                FaktaMenu.Destroy();
            });
            Add(FaktaMenu);
        }

        else if (FactNro == 3)
        {
            MultiSelectWindow FaktaMenu = new MultiSelectWindow("Kerätyt faktat", "fakta 1", "fakta 2", "fakta 3","takaisin");
            FaktaMenu.AddItemHandler(0, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact1Image);
            });
            FaktaMenu.AddItemHandler(1, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact2Image);
            });
            FaktaMenu.AddItemHandler(2, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact3Image);
            });
            FaktaMenu.AddItemHandler(3, delegate
            {
                PauseMenu();
                FaktaMenu.Destroy();
            });
            Add(FaktaMenu);
        }

        else if (FactNro == 4)
        {
            MultiSelectWindow FaktaMenu = new MultiSelectWindow("Kerätyt faktat", "fakta 1", "fakta 2", "fakta 3", "fakta4", "takaisin");
            FaktaMenu.AddItemHandler(0, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact1Image);
            });
            FaktaMenu.AddItemHandler(1, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact2Image);
            });
            FaktaMenu.AddItemHandler(2, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact3Image);
            });
            FaktaMenu.AddItemHandler(3, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact4Image);
            });
            FaktaMenu.AddItemHandler(4, delegate
            {
                PauseMenu();
                FaktaMenu.Destroy();
            });
            Add(FaktaMenu);
        }

        else
        {
            MultiSelectWindow FaktaMenu = new MultiSelectWindow("Kerätyt faktat", "fakta 1", "fakta 2", "fakta 3", "fakta4", "fakta5","takaisin");
            FaktaMenu.AddItemHandler(0, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact1Image);
            } );
            FaktaMenu.AddItemHandler(1, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact2Image);
            });
            FaktaMenu.AddItemHandler(2, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact3Image);
            });
            FaktaMenu.AddItemHandler(3, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact4Image);
            });
            FaktaMenu.AddItemHandler(4, delegate
            {
                FaktaMenu.Destroy();
                ShowAFact2(Fact5Image);
            });
            FaktaMenu.AddItemHandler(5, delegate
            {
                PauseMenu();
                FaktaMenu.Destroy();
            });
            Add(FaktaMenu);
        }
        Keyboard.Listen(Key.Enter, ButtonState.Pressed, delegate
        {
            fakta.Destroy();
            FactMenu();
        }, null
        );
    }

    void ShowAFact2 (Image Kuva)
    {
        fakta = new GameObject(300, 500);
        fakta.Position = player.Position;
        fakta.Image = Kuva;
        fakta.MaximumLifetime = TimeSpan.FromSeconds(20);
        Add(fakta);
    }

    void AddHealth(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        if(PlayerLife <100)
        {
            kohde.Destroy();
            PlayerLife.Value += 20;
            
        }
        else
        {
            MessageDisplay.Add("Sinulla on jo täydet elämät!");
        }
    }

    void Dead()
    {
        player.Destroy();
        MessageDisplay.Add("Kuolit");
        Timer.SingleShot(5, NextLevel);
    }
}

