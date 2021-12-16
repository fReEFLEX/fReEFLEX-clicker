using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.Linq;


namespace fReEFLEX_clicker
{
    public class ClickerGame : Game
    {
        //delegates
        public delegate void delegateBool(bool arg);
        public delegateBool delegateSerialState;

        //window
        const int widthDefault = 500;
        const int heightDefault = 300;

        //graphics
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        //input
        private readonly KeyboardListener keyboardListener;
        private bool isPortInput = false;

        //serial port
        private bool serialState = false;
        private Thread serialThread;

        //messages
        const string msgHelp = "F: toggle fullscreen\nC: toggle Back/White mode\nArrow Up, Plus: increase FPS by 10\nArrow Down, Minus: decrease FPS by 10\nP: connect to com port";
        const string msgDefault = "Photosensitive Warning:\nDo not look at the screen while clicking fast.\nPress \"H\" for help";
        private string currentMessage = "";
        private Vector2 msgPosition;
        private System.Timers.Timer msgTimer = new System.Timers.Timer(5000);

        //colors
        private Color colorDefault = new Color(50, 50, 50);
        private Color colorClick = new Color(200, 200, 200);
        private Color currentColor = new Color(50, 50, 50);

        //fps
        private UInt32 currentFps = 1000;
        private FPSCounter fpsCounter = new FPSCounter();

        public ClickerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //set to fixed fps
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / currentFps);

            //show mouse
            IsMouseVisible = true;

            //add input listener
            keyboardListener = new KeyboardListener();
            Components.Add(new InputListenerComponent(this, keyboardListener));
            keyboardListener.KeyPressed += HandleInput;

            //message timer
            msgTimer.Elapsed += ResetMsg;

            //delegates
            delegateSerialState = new delegateBool(UpdateSerialState);
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = widthDefault;
            graphics.PreferredBackBufferHeight = heightDefault;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            UpdateMsgPosition();
            ResetMsg(null, null);
            base.Initialize();
        }
        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            // Stop the threads
            if (serialThread != null)
                serialThread.Interrupt();
        }
        private void UpdateSerialState(bool state)
        {
            serialState = state;
            ResetMsg(null, null);
        }

        private void HandleInput(Object sender, KeyboardEventArgs args)
        {
            if (isPortInput)
            {
                isPortInput = false;
                string port = "";
                try
                {
                    port = SerialPort.GetPortNames()[int.Parse(args.Character.Value.ToString())];
                }
                catch (Exception)
                {
                    ShowMsg("invalid port selected: " + args.Character.Value.ToString());
                    return;
                }
                //Debug.WriteLine("opening com port: " + "COM" + SerialPort.GetPortNames().GetValue);
                ShowMsg("opening com port: " + port);
                serialThread = new Thread(() => { SerialDevice serialDevice = new SerialDevice(this, port); serialDevice.Run(); });
                serialThread.Start();
                return;
            }

            switch (args.Key)
            {
                case Keys.Up:
                case Keys.Add:
                case Keys.OemPlus:
                    SetFPS(Math.Min(5000, currentFps + 10));
                    break;
                case Keys.Down:
                case Keys.Subtract:
                case Keys.OemMinus:
                    SetFPS(Math.Max(10, currentFps - 10));
                    break;
                case Keys.Escape:
                    Exit();
                    break;
                case Keys.F:
                    ToggleFullscreen();
                    break;
                case Keys.H:
                    ShowMsg(msgHelp);
                    break;
                case Keys.C:
                    ToggleColors();
                    break;
                case Keys.P:
                    if (serialThread != null)
                    {
                        serialThread.Interrupt();
                        serialThread = null;
                    }
                    ShowMsg("select com port (press number):\n" + 
                        String.Join("; ", SerialPort.GetPortNames().Select((d, i) => i.ToString() + ": " + d.Trim()).ToArray()));
                    isPortInput = true;
                    break;
                default:
                    break;
            }
        }

        private void SetFPS(UInt32 fps)
        {
            this.currentFps = fps;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / this.currentFps);
            ShowMsg(this.currentFps.ToString() + " FPS");
        }

        private void ToggleFullscreen()
        {
            if (graphics.IsFullScreen)
            {
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
                graphics.PreferredBackBufferWidth = widthDefault;
                graphics.PreferredBackBufferHeight = heightDefault;

            }
            else
            {
                graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                graphics.ApplyChanges();
                graphics.IsFullScreen = true;
            }
            graphics.ApplyChanges();
            UpdateMsgPosition();
        }

        private void ToggleColors()
        {
            if (colorDefault.R == 50)
            {
                colorDefault = new Color(0, 0, 0);
                colorClick = new Color(255, 255, 255);
            } else
            {
                colorDefault = new Color(50, 50, 50);
                colorClick = new Color(200, 200, 200);
            }
        }

        protected void UpdateMsgPosition()
        {
            msgPosition = new Vector2(10f, Window.ClientBounds.Height - 110f);
        }

        private void ResetMsg(Object sender, System.Timers.ElapsedEventArgs e)
        {
            ShowMsg(msgDefault + "\nSerial Port: " + (serialState ? "connected" : "disconnected"));
            isPortInput = false;
        }

        public void ShowMsg(string message)
        {
            msgTimer.Stop();
            currentMessage = message;
            msgTimer.Start();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Fonts/RobotoMono-Medium");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                currentColor = colorClick;
            }
            else
            {
                currentColor = colorDefault;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(currentColor);
            spriteBatch.Begin();
            fpsCounter.DrawFps(gameTime, spriteBatch, font, new Vector2(10f, 10f), Color.MonoGameOrange);
            spriteBatch.DrawString(font, currentMessage, msgPosition, Color.MonoGameOrange);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class FPSCounter
    {
        private double frames = 0;
        private double elapsed = 0;
        private double last = 0;
        private double now = 0;
        const double msgFrequency = 0.5f;
        public string msg = "";

        public void DrawFps(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, Vector2 fpsDisplayPosition, Color fpsTextColor)
        {
            now = gameTime.TotalGameTime.TotalSeconds;
            elapsed = (double)(now - last);
            if (elapsed > msgFrequency)
            {
                UpdateMsg();
            }
            spriteBatch.DrawString(font, msg, fpsDisplayPosition, fpsTextColor);
            frames++;
        }

        private void UpdateMsg()
        {
            msg = "FPS: " + (frames / elapsed).ToString("#", System.Globalization.CultureInfo.InvariantCulture) +
                "\nframetime: " + (1000 * elapsed / Math.Max(1, frames)).ToString("0.0 ms", System.Globalization.CultureInfo.InvariantCulture);
            elapsed = 0;
            frames = 0;
            last = now;
        }
    }

    public class SerialDevice
    {
        const float captureRate = .1f;
        const int baudrate = 250000;

        private ClickerGame game;
        private SerialPort serialPort;
        private bool isClick = false;
        public SerialDevice(ClickerGame g1, String portName)
        {
            game = g1;
            serialPort = new SerialPort(portName, SerialDevice.baudrate);
            game.delegateSerialState(false);
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(SerialDevice.captureRate));
                    if (!serialPort.IsOpen)
                    {   //sleep a little longer
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                        SerialPort.GetPortNames();
                        TryConnect();
                        continue;
                    }

                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (!isClick)
                        {
                            SendClickedReceived();
                            isClick = true;
                        }
                    }
                    else if (isClick)
                    {
                        isClick = false;
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("closing serial thread");
                    if (serialPort.IsOpen)
                        serialPort.Dispose();
                    game.delegateSerialState(false);
                    break;
                }
            }
        }

        private void SendClickedReceived()
        {
            if (!serialPort.IsOpen)
            {
                return;
            }
            try { 
                serialPort.Write(new byte[1] { 0 }, 0, 1);
            } catch (Exception)
            {
                Debug.WriteLine("error sending click response");
                serialPort.Close();
                game.delegateSerialState(false);
            }
        }

        private void TryConnect()
        {
            try {
                serialPort.Open();
                game.delegateSerialState(true);
            } catch (Exception)
            {
                Debug.WriteLine("error opening com port " + serialPort.PortName);
                serialPort.Close();
                game.delegateSerialState(false);
            }
        }
    }
}
