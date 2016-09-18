using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class Uskontopeli : PhysicsGame
{

    PhysicsObject player;
    PhysicsObject Enemy1;
    PhysicsObject ammus;
    PhysicsObject Goal;
    PhysicsObject Fact1;

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

    int KenttaNumero = 1;
    int FactNro = 0;
    public override void Begin()
    {
        NextLevel();
        
    }
    void NextLevel()
    {
        ClearAll();

        if (KenttaNumero == 1) Luokentta("Kartta1");
        else if (KenttaNumero == 2) Luokentta("mapui2");
        else if (KenttaNumero == 3) Luokentta("Kartta3");
        else if (KenttaNumero == 4) Luokentta("Kartta4");
        else if (KenttaNumero == 5) Exit();
    }
    void Luokentta(string KenttanNimi)
    {


        ColorTileMap kentta = ColorTileMap.FromLevelAsset(KenttanNimi);
        kentta.SetTileMethod("#FFFF00B2", Addplayer);
        kentta.SetTileMethod("#FF000000", AddWall);
        kentta.SetTileMethod("#FF0015FF", CreateEnemy1);
        kentta.SetTileMethod("#FFFF000C", AddGoal);
        kentta.SetTileMethod("#FFFAFF07", AddFact1);
        kentta.SetTileMethod("#FFFAFF08", AddAbility1);

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

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed,delegate
        {
            Pause();
            PauseMenu();
        },null);
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
            
        }

    }

    void Aim (AnalogState hiirenLiike)
    {
        if (playerWeapon1 == null) return;
      Vector suunta = (Mouse.PositionOnWorld - playerWeapon1.AbsolutePosition).Normalize();
      playerWeapon1.Angle = suunta.Angle;
    
    }

    void CreateEnemy1 (Vector paikka, double korkeus, double leveys)
    {
        Enemy1 = new PhysicsObject(75, 100);
        Enemy1.Color = Color.HotPink;
        Enemy1.Shape = Shape.Rectangle;
        Enemy1.Position = paikka;
        Enemy1.CanRotate = false;
        Enemy1.Tag = "Vihu";
        Enemy1.CollisionIgnoreGroup = 1;
        Enemy1.Animation = new Animation(Enemy1Idle);
        Enemy1.Animation.FPS = 2;
        Enemy1.Animation.Start();
        Add(Enemy1);
        

        FollowerBrain Enemy1brain = new FollowerBrain("Pelaaja");
        Enemy1brain.Speed = 350;
        Enemy1brain.DistanceFar = 500;
        Enemy1.Brain = Enemy1brain;
        Enemy1brain.Active = true;

    }

    void SeuraavaKentta(PhysicsObject tormaaja, PhysicsObject kohde)
    {
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
    
    void AddFact1(Vector paikka, double korkeus, double leveys)
    {
        Fact1 = new PhysicsObject(100, 100);
        Fact1.Color = Color.Yellow;
        Fact1.Position = paikka;
        Fact1.IgnoresPhysicsLogics = true;
        Fact1.CollisionIgnoreGroup = 1;
        Fact1.MakeStatic();
        Add(Fact1);

        AddCollisionHandler(player, Fact1, ShowAFact);
    }

    void AddAbility1(Vector paikka, double korkeus, double leveys)
    {
        PhysicsObject PickA1 = new PhysicsObject(100, 100);
        PickA1.Color = Color.Green;
        PickA1.Position = paikka;
        PickA1.MakeStatic();
        Add(PickA1);
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
            fakta.Image = Pappikuva;
        }

        kohde.Destroy();

        Keyboard.Listen(Key.Enter, ButtonState.Pressed, delegate
        {
            fakta.Destroy();
        }
        ,null);
    }
    
       void PauseMenu()
    {

        MultiSelectWindow PauseValikko = new MultiSelectWindow("Peli on pysäytty", "Jatka peliä", "Kerätyt faktat", "Päävalikko");
        PauseValikko.AddItemHandler(0, Pause);
        PauseValikko.AddItemHandler(1, FactMenu);
        PauseValikko.AddItemHandler(2, Exit);
        Add(PauseValikko);
    }

    void FactMenu()
    {
        if (FactNro == 0)
        {
            MultiSelectWindow FaktaMenu = new MultiSelectWindow("Et ole löytänyt yhtään faktaa vielä", "Jatka peliä");
            FaktaMenu.AddItemHandler(0, delegate
            {
                Pause();
            }
            );
            Add(FaktaMenu);
        }
       else if (FactNro == 1)
        {
            MultiSelectWindow FaktaMenu = new MultiSelectWindow("Kerätyt faktat", "fakta 1", "takaisin");
            FaktaMenu.AddItemHandler(0, delegate
            {
                ShowAFact2(Fact1Image);
            }
            );
            FaktaMenu.AddItemHandler(1, PauseMenu);
            Add(FaktaMenu);
        }

       else if (FactNro == 2)
        { 
        MultiSelectWindow FaktaMenu = new MultiSelectWindow("Kerätyt faktat", "fakta 1", "fakta 2", "takaisin");
            FaktaMenu.AddItemHandler(0, delegate
            {
                ShowAFact2(Fact1Image);
            }
            );
            FaktaMenu.AddItemHandler(1, delegate
            {
                ShowAFact2(Pappikuva);
            });
            FaktaMenu.AddItemHandler(2, Pause);
            Add(FaktaMenu);
        }
    }

    void ShowAFact2 (Image Kuva)
    {
        GameObject fakta = new GameObject(300, 500);
        fakta.Position = player.Position;
        fakta.Image = Kuva;
        Add(fakta);

        Keyboard.Listen(Key.Enter, ButtonState.Pressed, delegate
        {
            fakta.Destroy();
            FactMenu();
        }
, null);
    }

}
