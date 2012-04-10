using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Razcers
{
    public class InputState : GameComponent
    {

        public enum InputMode
        {
            Standard,
            Lefty,
            Simple,
            Advanced,
            Custom
        }

        public struct HoldTimer
        {
            Buttons button;
            TimeSpan fireTime;
            TimeSpan currentTime;
            EventHandler fireMethod;
        }

        public GamePadState[] gamePadStates { get; set;}
        public GamePadState[] prevGamePadStates { get; set; }
        protected int gamePadsConnected { get; set; }
        protected int prevGamePadsConnected { get; set; }

        public IList<HoldTimer> timers { get; set; }

#if WINDOWS

        public KeyboardState keyboardState { get; set; }
        protected KeyboardState prevKeyboardState { get; set; }
        public MouseState mouseState { get; set; }
        protected MouseState prevMouseState { get; set; }

#endif
        public InputState(Game game) : base (game)
        {
            gamePadStates = new GamePadState[4];
            prevGamePadStates = new GamePadState[4];

            for (int i = 0; i < 4; i++)
            {
                gamePadStates[i] = new GamePadState();
                prevGamePadStates[i] = new GamePadState();
                if (gamePadStates[i].IsConnected) gamePadsConnected++;
                
            }
            prevGamePadsConnected = gamePadsConnected;
#if WINDOWS
            prevKeyboardState = new KeyboardState();
            keyboardState = new KeyboardState();
            prevMouseState = new MouseState();
            mouseState = new MouseState();

#endif

        }

        public override void Update(GameTime gameTime)
        {
            prevGamePadsConnected = gamePadsConnected;
            gamePadsConnected = 0;
            for (int i = 0; i < 4; i++)
            {
                prevGamePadStates[i] = gamePadStates[i];
                gamePadStates[i] = GamePad.GetState((PlayerIndex)i);
                if (gamePadStates[i].IsConnected) gamePadsConnected++;
            }
            if (gamePadsConnected > prevGamePadsConnected) { }
            else if (gamePadsConnected < prevGamePadsConnected) { }
#if WINDOWS
            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
#endif

        }

        public GamePadState GetGamePadState(PlayerIndex player)
        {
            return (gamePadStates[(int)player]);
        }


#if WINDOWS
        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
                                            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (keyboardState.IsKeyDown(key) &&
                        prevKeyboardState.IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }
#endif


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        /// 
        public bool IsNewButtonPress(Buttons button, PlayerIndex controllingPlayer)
        {
            return IsNewButtonPress(button, controllingPlayer, out controllingPlayer);
        }


        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
                                                     out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (gamePadStates[i].IsButtonDown(button) &&
                        prevGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }

        public Vector2 IsMoveDirection(PlayerIndex player)
        {
#if WINDOWS
            if (keyboardState.IsKeyDown(Keys.Left))
                return -Vector2.UnitX;
            else if (keyboardState.IsKeyDown(Keys.Right))
                return Vector2.UnitX;
            else if (keyboardState.IsKeyDown(Keys.Up))
                return Vector2.UnitY;
            else if (keyboardState.IsKeyDown(Keys.Down))
                return -Vector2.UnitY;
#endif
            if (gamePadsConnected > 0)
            {
                if (gamePadStates[(int)player].DPad.Left == ButtonState.Pressed)
                    return -Vector2.UnitX;
                else if (gamePadStates[(int)player].DPad.Right == ButtonState.Pressed)
                    return Vector2.UnitX;
                else if (gamePadStates[(int)player].DPad.Up == ButtonState.Pressed)
                    return Vector2.UnitY;
                else if (gamePadStates[(int)player].DPad.Down == ButtonState.Pressed)
                    return -Vector2.UnitY;
                else
                    return Orthoganize(gamePadStates[(int)player].ThumbSticks.Left);
            }
            else
            {

            }
            return Vector2.Zero;
        }
        /// <summary>
        /// Checks for a "menu select" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return //IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        public float GetPitch(PlayerIndex player, InputMode inputMode)
        {
            switch (inputMode)
            {
                case (InputMode.Advanced):
                    return GetPitchAdvanced(player);
                case (InputMode.Standard):
                    return GetPitchStandard(player);
                case (InputMode.Lefty):
                    return GetPitchLefty(player);
                case (InputMode.Simple):
                    return GetPitchSimple(player);
                case (InputMode.Custom):
                    return GetPitchCustom(player);
                default:
                    return 0;
            }
        }

        private float GetPitchAdvanced(PlayerIndex player)
        {
            if (gamePadStates[(int)player].IsConnected)
            {
                return
                    (gamePadStates[(int)player].ThumbSticks.Left.Y +
                    gamePadStates[(int)player].ThumbSticks.Right.Y);
            }
            else
            {
                return
                    (keyboardState.IsKeyDown(Keys.Up) ? 2 : 0) +
                    (keyboardState.IsKeyDown(Keys.Down) ? -2 : 0);
            }
        }
        private float GetPitchStandard(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float GetPitchLefty(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float GetPitchSimple(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float GetPitchCustom(PlayerIndex player)
        {
            throw new NotImplementedException();
        }

        public float GetRoll(PlayerIndex player, InputMode inputMode)
        {
            switch (inputMode)
            {
                case (InputMode.Advanced):
                    return GetRollAdvanced(player);
                case (InputMode.Standard):
                    return GetRollStandard(player);
                case (InputMode.Lefty):
                    return GetRollLefty(player);
                case (InputMode.Simple):
                    return GetRollSimple(player);
                case (InputMode.Custom):
                    return GetRollCustom(player);
                default:
                    return 0;
            }
        }

        private float GetRollAdvanced(PlayerIndex player)
        {
            if (gamePadStates[(int)player].IsConnected)
            {
                return
                    (gamePadStates[(int)player].ThumbSticks.Left.Y -
                    gamePadStates[(int)player].ThumbSticks.Right.Y);
            }
            else
            {
                return
                    (keyboardState.IsKeyDown(Keys.Right) ? 2 : 0) +
                    (keyboardState.IsKeyDown(Keys.Left) ? -2 : 0);
            }
        }
        private float GetRollStandard(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float GetRollLefty(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float GetRollSimple(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float GetRollCustom(PlayerIndex player)
        {
            throw new NotImplementedException();
        }

        public float GetThrust(PlayerIndex player, InputMode inputMode)
        {
            switch (inputMode)
            {
                case (InputMode.Advanced):
                    return GetThrustAdvanced(player);
                case (InputMode.Standard):
                    return GetThrustStandard(player);
                case (InputMode.Lefty):
                    return GetThrustLefty(player);
                case (InputMode.Simple):
                    return GetThrustSimple(player);
                case (InputMode.Custom):
                    return GetThrustCustom(player);
                default:
                    return 0;
            }
        }
        private float GetThrustAdvanced(PlayerIndex player)
        {
            if (gamePadStates[(int)player].IsConnected)
            {
                return
                    (gamePadStates[(int)player].Triggers.Right +
                    gamePadStates[(int)player].Triggers.Left);
            }
            else
            {
                return
                    (keyboardState.IsKeyDown(Keys.LeftControl) ? 1 : 0) +
                    (keyboardState.IsKeyDown(Keys.LeftAlt) ? 1 : 0);
            }
        }
        private float GetThrustStandard(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float GetThrustLefty(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float GetThrustSimple(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float GetThrustCustom(PlayerIndex player)
        {
            throw new NotImplementedException();
        }

        public float LeftAilervator(PlayerIndex player, InputMode inputMode)
        {
            switch (inputMode)
            {
                case (InputMode.Advanced):
                    return LeftAilervatorAdvanced(player);
                case (InputMode.Standard):
                    return LeftAilervatorStandard(player);
                case (InputMode.Lefty):
                    return LeftAilervatorLefty(player);
                case (InputMode.Simple):
                    return LeftAilervatorSimple(player);
                case (InputMode.Custom):
                    return LeftAilervatorCustom(player);
                default:
                    return 0;
            }

        }

        private float LeftAilervatorAdvanced(PlayerIndex player)
        {
            if (gamePadStates[(int)player].IsConnected)
            {
                return
                    (gamePadStates[(int)player].ThumbSticks.Left.Y * 0.4f);
            }
            else
            {
                return
                    (keyboardState.IsKeyDown(Keys.Left) ? -0.4f : 0) +
                    (keyboardState.IsKeyDown(Keys.Up) ? 0.4f : 0) +
                    (keyboardState.IsKeyDown(Keys.Down) ? -0.4f : 0);
            }
        }
        private float LeftAilervatorStandard(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float LeftAilervatorLefty(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float LeftAilervatorSimple(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float LeftAilervatorCustom(PlayerIndex player)
        {
            throw new NotImplementedException();
        }

        public float RightAilervator(PlayerIndex player, InputMode inputMode)
        {
            switch (inputMode)
            {
                case (InputMode.Advanced):
                    return RightAilervatorAdvanced(player);
                case (InputMode.Standard):
                    return RightAilervatorStandard(player);
                case (InputMode.Lefty):
                    return RightAilervatorLefty(player);
                case (InputMode.Simple):
                    return RightAilervatorSimple(player);
                case (InputMode.Custom):
                    return RightAilervatorCustom(player);
                default:
                    return 0;
            }

        }

        private float RightAilervatorAdvanced(PlayerIndex player)
        {
            if (gamePadStates[(int)player].IsConnected)
            {
                return
                    (gamePadStates[(int)player].ThumbSticks.Right.Y * 0.4f);
            }
            else
            {
                return
                    (keyboardState.IsKeyDown(Keys.Right) ? 0.4f : 0) +
                    (keyboardState.IsKeyDown(Keys.Up) ? 0.4f : 0) +
                    (keyboardState.IsKeyDown(Keys.Down) ? -0.4f : 0);
            }
        }
        private float RightAilervatorStandard(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float RightAilervatorLefty(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float RightAilervatorSimple(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        private float RightAilervatorCustom(PlayerIndex player)
        {
            throw new NotImplementedException();
        }
        
        public Vector3 Orthoganize(Vector3 vect3)
        {
            float dotProduct;
            Vector3 maxDotProducter;

            dotProduct = Vector3.Dot(vect3, Vector3.Up);
            maxDotProducter = Vector3.Up;

            if (Vector3.Dot(vect3, Vector3.Down) > dotProduct)
            {
                dotProduct = Vector3.Dot(vect3, Vector3.Down);
                maxDotProducter = Vector3.Down;
            }
            if (Vector3.Dot(vect3, Vector3.Left) > dotProduct)
            {
                dotProduct = Vector3.Dot(vect3, Vector3.Left);
                maxDotProducter = Vector3.Left;
            }
            if (Vector3.Dot(vect3, Vector3.Right) > dotProduct)
            {
                dotProduct = Vector3.Dot(vect3, Vector3.Right);
                maxDotProducter = Vector3.Right;
            }
            if (Vector3.Dot(vect3, Vector3.Forward) > dotProduct)
            {
                dotProduct = Vector3.Dot(vect3, Vector3.Forward);
                maxDotProducter = Vector3.Forward;
            }
            if (Vector3.Dot(vect3, Vector3.Backward) > dotProduct)
            {
                dotProduct = Vector3.Dot(vect3, Vector3.Backward);
                maxDotProducter = Vector3.Backward;
            }

            if (vect3 == Vector3.Zero)
                maxDotProducter = Vector3.Zero;

            return maxDotProducter;
        }
        public Vector2 Orthoganize(Vector2 vect2)
        {
            float dotProduct;
            Vector2 maxDotProducter;

            dotProduct = Vector2.Dot(vect2, Vector2.UnitY);
            maxDotProducter = Vector2.UnitY;

            if (Vector2.Dot(vect2, -Vector2.UnitY) > dotProduct)
            {
                dotProduct = Vector2.Dot(vect2, -Vector2.UnitY);
                maxDotProducter = -Vector2.UnitY;
            }
            if (Vector2.Dot(vect2, -Vector2.UnitX) > dotProduct)
            {
                dotProduct = Vector2.Dot(vect2, -Vector2.UnitX);
                maxDotProducter = -Vector2.UnitX;
            }
            if (Vector2.Dot(vect2, Vector2.UnitX) > dotProduct)
            {
                dotProduct = Vector2.Dot(vect2, Vector2.UnitX);
                maxDotProducter = Vector2.UnitX;
            }

            if (vect2 == Vector2.Zero)
                maxDotProducter = Vector2.Zero;

            return maxDotProducter;
        }


    }
}
