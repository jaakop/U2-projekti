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

    public override void Begin()
    {
        AddPlayer();
        AddControlls();

        //Camera.Follow(player);
        Camera.Zoom(0.75);

    }



    void AddPlayer()
    {
        player = new PhysicsObject(75, 100);
        player.Shape = Shape.Rectangle;
        player.Color = Color.HotPink;

        Add(player);

    }

    void AddControlls()
    {
        Keyboard.Listen(Key.A, ButtonState.Down, MovePlayer, null, new Vector(-1000, 0));
        Keyboard.Listen(Key.A, ButtonState.Released, delegate
        {
            player.Stop();
        }, null );
        Keyboard.Listen(Key.D, ButtonState.Down, MovePlayer, null, new Vector(1000, 0));
        Keyboard.Listen(Key.D, ButtonState.Released, delegate
        {
            player.Stop();
        }, null);
        Keyboard.Listen(Key.W, ButtonState.Down, MovePlayer, null, new Vector(0, 1000));
        Keyboard.Listen(Key.W, ButtonState.Released, delegate
        {
            player.Stop();
        }, null);
        Keyboard.Listen(Key.S, ButtonState.Down, MovePlayer, null, new Vector(0, -1000));
        Keyboard.Listen(Key.S, ButtonState.Released, delegate
        {
            player.Stop();
        }, null);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
    void MovePlayer(Vector Vektori )
    {
        player.Push(Vektori);
    }
}
