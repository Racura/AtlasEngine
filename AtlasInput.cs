using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if MONOGAME
using Microsoft.Xna.Framework.Input.Touch;
#endif

namespace AtlasEngine
{
    public class AtlasInput
    {
        private GamePadState[] gamepads;

        private KeyboardState keyboard;
        private KeyboardState lastKeyboard;

        private MouseState mouse;
        private MouseState lastMouse;

        private List<AtlasTouchPosition> touches;

        public AtlasInput()
            : base()
        {
            gamepads = new GamePadState[8];
            for (int i = 0; i < gamepads.Length; i++)
                gamepads[i] = new GamePadState();

            lastKeyboard = keyboard = new KeyboardState();
            mouse = lastMouse = new MouseState();

            touches = new List<AtlasTouchPosition>();

#if MONOGAME
            TouchPanel.EnableMouseTouchPoint = true;
#endif
#if XNA
            touches.Add(new AtlasTouchPosition(
                    new TouchLocation(new Vector2(mouse.X, mouse.Y), TouchLocationState.Invalid)
            ));
#endif


        }

        internal void Update()
        {
            lastKeyboard = keyboard;
            lastMouse = mouse;

            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();
            for (int i = 0; i < gamepads.Length/2; i++){
                gamepads[gamepads.Length / 2 + i] = gamepads[i];
                gamepads[i] = GamePad.GetState((PlayerIndex)i);
            }
            /*
            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].State == TouchLocationState.Released)
                {
                    touches.Remove(touches[i]);
                    i--;
                }
            }
            */
 
#if MONOGAME
            var touch = TouchPanel.GetState();
            foreach (var t in touch)
            {
                if (t.State != TouchLocationState.Pressed)
                {
                    for (int i = 0; i < touches.Count; i++)
                    {
                        if (touches[i].Id == t.Id)
                        {
                            touches[i].Update(t);
                        }
                    }
                }
                else
                    touches.Add(new TouchPosition(t));
            }
#endif
#if XNA

            foreach(var t in touches){

                if (t.Id == -1)
                {
                    if (mouse.LeftButton == ButtonState.Released
                        && lastMouse.LeftButton == ButtonState.Released)
                    {
                        t.Update(
                            new TouchLocation(new Vector2(mouse.X, mouse.Y), TouchLocationState.Invalid)
                        );
                        t.SetOwner(null);
                    }
                    else if (mouse.LeftButton == ButtonState.Pressed && lastMouse.LeftButton == ButtonState.Released)
                    {
                        t.Update(
                            new TouchLocation(new Vector2(mouse.X, mouse.Y), TouchLocationState.Pressed)
                        );
                    }
                    else if (mouse.LeftButton == ButtonState.Released)
                    {
                        t.Update(
                            new TouchLocation(new Vector2(mouse.X, mouse.Y), TouchLocationState.Released)
                        );
                    }
                    else
                    {
                        t.Update(
                            new TouchLocation(new Vector2(mouse.X, mouse.Y), TouchLocationState.Moved)
                        );
                    }
                }
            }
#endif

            //touches = TouchPanel.GetState();
        }

        public List<AtlasTouchPosition> GetTouchCollection() { return touches; }

        public KeyboardState GetKeyboard() { return keyboard; }
        public KeyboardState GetOldKeyboard() { return lastKeyboard; }

        public MouseState GetMouse() { return mouse; }
        public MouseState GetOldMouse() { return lastMouse; }

        public GamePadState GetGamePad(int player) { return gamepads[player]; }
        public GamePadState GetGamePad(PlayerIndex player) { return gamepads[(int)player]; }
        public GamePadState GetOldGamePad(int player) { return gamepads[player + 4]; }
        public GamePadState GetOldGamePad(PlayerIndex player) { return gamepads[(int)player + 4]; }


        public bool IsKeyDown(Keys key) { return keyboard.IsKeyDown(key); }
        public bool IsKeyJustPressed(Keys key) { return keyboard.IsKeyDown(key) && !lastKeyboard.IsKeyDown(key); }
        public bool IsKeyJustReleased(Keys key) { return !keyboard.IsKeyDown(key) && lastKeyboard.IsKeyDown(key); }

        
    }

    public class AtlasTouchPosition
    {
        private TouchLocation _t;
        private TouchLocation _last;

        private object _owner;

        public Vector2 Position { get { return _t.Position; } }
        public Vector2 PreviousPosition { get { return _last.Position; } }
        public TouchLocationState State { get { return _t.State; } }
        public float Pressure { get { return _t.Pressure; } }
        public int Id { get { return _t.Id; } }
        public object Owner { get { return _owner; } }
        public bool HasOwner { get { return _owner != null; } }

        public AtlasTouchPosition(TouchLocation t)
        {
            _last = _t = t;
        }

        internal void Update(TouchLocation t)
        {
            _last = _t;
            _t = t;
        }

        public void SetOwner(object owner)
        {
            this._owner = owner;
        }
    }

#if XNA
    public struct TouchLocation
    {
        public Vector2 Position;
        public TouchLocationState State;
        public float Pressure;
        public int Id;



        public TouchLocation(Vector2 position, TouchLocationState state)
        {
            this.Position = position;
            this.State = state;
            this.Pressure = 1;
            this.Id = -1;
        }
    }

    public enum TouchLocationState
    {
        Invalid,
        Released,
        Pressed,
        Moved
    }
#endif
}
