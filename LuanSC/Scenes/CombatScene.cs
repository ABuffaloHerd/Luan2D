using LuanSC.Data;
using LuanSC.Data.Components;
using SadConsole.Components;
using SadConsole.Renderers;
using SadConsole.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class CombatScene : Scene
    {
        // Surface is where the gameobjects are drawn
        private LayeredScreenSurface surface;

        // overlay is where effects and the cursor is drawn

        private CellSurface overlay;

        // Controls
        private ControlsConsole controls;
        private HUD hud;

        private CombatSettings settings;
        private FightFeed fightFeed;
        private ScreenSurface order;

        private Rectangle view;

        private bool overlayVisible = true;

        public CombatScene(SceneManager manager, CombatSettings settings) : base(manager)
        {
            // Set up surfaces and borders
            this.settings = settings;

            /// Main surface for game objects. Has the entity manager attached to it.
            surface = new(settings.Width, settings.Height);
            surface.Position = new(1, 1);
            //surface.FontSize *= settings.FontScale;
            surface.Resize(settings.Width, settings.Height, false);

            /// Overlay surface for effects and cursor and range patterns
            //overlay = new(settings.Width, settings.Height);
            //overlay.Position = new(1, 1);
            //overlay.FontSize *= settings.FontScale;
            //overlay.Resize(settings.Width, settings.Height, false);

            // while the warning here is sensible, layered console is supposed to come with a layered surface so if it doesn't then fuck what now
            overlay = surface.GetSadComponent<LayeredSurface>().Create();

            // setup the view rectangle to be the size of the surface
            view = new(0, 0, surface.Width, surface.Height);

            // now sync view with the arena and overlay
            surface.ViewWidth = view.Width;
            surface.ViewHeight = view.Height;


            Border.BorderParameters parameters = Border.BorderParameters.GetDefault();
            ShapeParameters shapeParams = ShapeParameters.CreateStyledBoxThick(Color.White);
            parameters.ChangeBorderStyle(shapeParams);
            parameters.AddTitle("Arena");
            parameters.TitleBackground = Color.White;
            parameters.TitleForeground = Color.Red;
            parameters.ChangeBorderForegroundColor(Color.Gray);
            new Border(surface, parameters);

            Children.Add(surface);


            // CONTROLS
            controls = new(21, 21);
            controls.Position = new(46, GameSettings.GAME_HEIGHT / 2 + 1);

            parameters = Border.BorderParameters.GetDefault();
            parameters.AddTitle("Controls");
            new Border(controls, parameters);

            Children.Add(controls);

            // HUD
            hud = new(21, 20);
            hud.Position = new(46, 1);
            parameters = Border.BorderParameters.GetDefault();
            parameters.AddTitle("HUD");
            new Border(hud, parameters);

            Children.Add(hud);

            // FIGHT FEED
            fightFeed = new FightFeed(20, 20);
            fightFeed.Position = new((controls.Position.X + controls.Width) * 2 + 4, 2); // y is 1 to accomodate border, x*2 to accommodate half font size

            Children.Add(fightFeed);

            // TURN ORDER
            order = new(20, 21);
            order.Position = new(hud.Position.X + order.Width + 3, hud.Position.Y + order.Height + 1);
            parameters = Border.BorderParameters.GetDefault();
            parameters.AddTitle("Turn Order");
            new Border(order, parameters);

            Children.Add(order);


            // make sure that only the scene itself is focused
            foreach (ScreenSurface child in Children)
            {
                child.UseKeyboard = false;
                child.IsFocused = false;
                child.FocusOnMouseClick = false;
            }

            // REALLY make sure this scene is focused
            this.IsFocused = true;

            // subscribe to my youtube channel (the event bus)
            combatEventHandler = HandleCombatEvent;
            Subscribe();

            // Set up game objects and entities
            InitObjects(settings);

        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            if (currentControlledGameObject is null) return;
            // if there is an AI component run that instead

            // Refresh the overlay if it's visible and is dirty
            if (overlayVisible)
                UpdateOverlay();

            hud.UpdateHP(currentControlledGameObject.GetComponent<HPComponent>());
            hud.UpdateMP(currentControlledGameObject.GetComponent<MPComponent>());
            hud.UpdateSP(currentControlledGameObject.GetComponent<SPComponent>());
            hud.UpdateOD(currentControlledGameObject.GetComponent<ODComponent>());
        }

        private void UpdateOverlay()
        {
            overlay.Clear();

            // This is only used for pattern rendering for now
            // Get current pattern from active controllable object.

            // Check that it is controllable object's turn
            if(currentControlledGameObject.GetComponent<ControllableComponent>() is null) return;

            // check for a weapon component
            if (currentControlledGameObject is null) return;
            WeaponComponent w = currentControlledGameObject.GetComponent<WeaponComponent>();
            if (w == null) return;

            var pattern = w.Weapon.Range;
            foreach (var cell in pattern.GetRotated(currentControlledGameObject.Direction))
            {
                int drawX = currentControlledGameObject.Position.X + cell.X;
                int drawY = currentControlledGameObject.Position.Y + cell.Y;
                if (drawX >= 0 && drawX < overlay.Width && drawY >= 0 && drawY < overlay.Height)
                {
                    ColoredGlyph g = new(Color.Yellow, Color.Transparent, 'X');
                    overlay.SetCellAppearance(drawX, drawY, g);
                }
            }
        }
    }
}
