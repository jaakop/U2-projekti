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
    }



    void AddPlayer()
    {
        player = new PhysicsObject(10, 10);
        player.Shape = Shape.Rectangle;
        player.Color = Color.HotPink;

        Add(player);

    }

    void AddControlls()
    {
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

}
