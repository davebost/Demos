using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SilverlightAndXNA
{
    public partial class GamePage : PhoneApplicationPage
    {
        GameTimer timer;
        ContentManager content;
        SpriteBatch spriteBatch;
        Texture2D texture;
        Vector2 spritePosition;
        Vector2 spriteSpeed = new Vector2(100.0f, 100.0f);

        Texture2D redTexture;
        Texture2D greenTexture;
        Texture2D blueTexture;

        int spriteHeight;
        int spriteWidth;

        // For rendering the XAML onto a Texture
        UIElementRenderer elementRenderer;

        public GamePage()
        {
            InitializeComponent();

            // Get the application's ContentManager
            content = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            // Use the LayoutUpdated event to know when the page layout
            // has completed so we can create the UIElementRenderer
            LayoutUpdated += new EventHandler(GamePage_LayoutUpdated);
        }

        void GamePage_LayoutUpdated(object sender, EventArgs e)
        {
            // Create the UIElementRenderer to draw the XAML page to a Texture

            // Check for 0 because when we navigate away the LayoutPage event
            // is raised but ActualWidth and ActualHeight will be 0 in that case
            if (ActualWidth > 0 && ActualHeight > 0 && elementRenderer == null)
            {
                elementRenderer = new UIElementRenderer(this, (int)ActualWidth, (int)ActualHeight);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            // TODO: use this.content to load your game content here
            if (null == texture)
            {
                redTexture = content.Load<Texture2D>("redRect");
                greenTexture = content.Load<Texture2D>("greenRect");
                blueTexture = content.Load<Texture2D>("blueRect");

                // Start with the red rectangle
                texture = redTexture;
            }

            spritePosition.X = 0;
            spritePosition.Y = 0;

            spriteHeight = texture.Height;
            spriteWidth = texture.Width;


            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            // TODO: Add your update logic here

            // Move the sprite around.
            UpdateSprite(e);
        }

        void UpdateSprite(GameTimerEventArgs e)
        {
            // Move the sprite by speed, scaled by elapsed time.
            spritePosition += spriteSpeed * (float)e.ElapsedTime.TotalSeconds;

            int MinX = 0;
            int MinY = 0;
            int MaxX = SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width - texture.Width;
            int MaxY = SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - texture.Height;

            // Check for bounce.
            if (spritePosition.X > MaxX)
            {
                spriteSpeed.X *= -1;
                spritePosition.X = MaxX;
            }

            else if (spritePosition.X < MinX)
            {
                spriteSpeed.X *= -1;
                spritePosition.X = MinX;
            }

            if (spritePosition.Y > MaxY)
            {
                spriteSpeed.Y *= -1;
                spritePosition.Y = MaxY;
            }
            else if (spritePosition.Y < MinY)
            {
                spriteSpeed.Y *= -1;
                spritePosition.Y = MinY;
            }

        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.Black);

            // Render the Silverlight controls using the UIElementRenderer
            elementRenderer.Render();


            // Draw the sprite
            spriteBatch.Begin();

            // Using the texture from the UIElementRenderer
            // draw the Silverlight controls to the screen
            spriteBatch.Draw(elementRenderer.Texture, Vector2.Zero, Color.White); 
            
            spriteBatch.Draw(texture, spritePosition, Color.White);
            spriteBatch.End();
        }

        private void ColorPanelToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Visibility.Visible == ColorPanel.Visibility)
            {
                ColorPanel.Visibility = System.Windows.Visibility.Collapsed;
            } else {
                ColorPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void redButton_Click(object sender, RoutedEventArgs e)
        {
            texture = redTexture;
        }

        private void greenButton_Click(object sender, RoutedEventArgs e)
        {
            texture = greenTexture;
        }

        private void blueButton_Click(object sender, RoutedEventArgs e)
        {
            texture = blueTexture;
        }
    }
}
