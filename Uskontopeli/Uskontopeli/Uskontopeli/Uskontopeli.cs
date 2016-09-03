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

    AssaultRifle playerWeapon1;

    private Image[] playerWalkDown = LoadImages("PappiAnimA1", "PappiAnimA2", "PappiAnimA1","PappiAnimA3");
    private Image[] playerWalkUp = LoadImages("PappiAnimY1", "PappiAnimY2");
    private Image[] playerWalkLeft = LoadImages("PappiAnimV1", "PappiAnimV2", "PappiAnimV1", "PappiAnimV3");
    private Image[] playerWalkRight = LoadImages("PappiAnimO1", "PappiAnimO2", "PappiAnimO1", "PappiAnimO3");
    private Image[] playerIdle = LoadImages("PappiAnimA1", "Pappi2");


    Image Pappikuva = LoadImage("PappiAnimA1");

    public override void Begin()
    {
        AddPlayer();
        AddControlls();

        Camera.Follow(player);
        Camera.Zoom(1);

    }

    void AddPlayer()
    {
        player = new PhysicsObject(75, 100);
        player.Shape = Shape.Rectangle;
        player.Color = Color.HotPink;
        player.Image = Pappikuva;
        player.MaxVelocity = 1000;

        playerWeapon1 = new AssaultRifle(30, 10);
        playerWeapon1.ProjectileCollision = Hit;
        playerWeapon1.InfiniteAmmo = true;
        playerWeapon1.Power.Value = 2000;
        player.Add(playerWeapon1);
        

        Add(player);

    }

    void AddControlls()
    {
       

        Keyboard.Listen(Key.S, ButtonState.Down, delegate
        {
            MovePlayer(new Vector(0, -1000));
          
        }, null);
        Keyboard.Listen(Key.S, ButtonState.Pressed, delegate
        {
            player.Animation = new Animation(playerWalkDown);
            player.Animation.FPS = 10;
            player.Animation.Start();

        }, null);
        Keyboard.Listen(Key.S, ButtonState.Down, delegate
        {
            MovePlayer(new Vector(0, -1000));

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
            MovePlayer(new Vector(1000, 0));

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
            MovePlayer(new Vector(0, 1000));

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
            MovePlayer(new Vector(-1000, 0));

        }, null);
        Keyboard.Listen(Key.A, ButtonState.Released, delegate
        {
            IdlePlayer();
            player.Stop();
        }, null);

        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, Shoot, "shoot", playerWeapon1);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
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

    }

    void Shoot(AssaultRifle weapon)
    {
        PhysicsObject ammus = weapon.Shoot();
        if(ammus != null)
        {
            ammus.Size *= 3;
            ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
            
        }

    }

}
