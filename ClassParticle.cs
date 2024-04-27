using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.Win32.SafeHandles;
using Color = System.Drawing.Color;
using Timer = System.Windows.Forms.Timer;

namespace LibParticle
{
    public class Particle
    {

        private float x;
        private float y;
        private float size;
        private float speedX;
        private float speedY;
        private static Color[] _color;
        private Color color;

        private int shapeType = 5;
        //private int shapeType; // 0-4: Circles, 5: Small Triangles, 6: Lines
        private static float glowsize;
        private static int blursteps;
        public static int maxparticles;
        public static int changetime;
        public enum Shapetype
        {
            circle,
            Lines,
            Triangle,
            Mixed
        }
        public enum glowtype
        {
            Neon,
            Bubble,
            Shadow,
            Neon_Bubble,
            None
        }

        private static Shapetype myShapetype;
        private static glowtype myglowtype;
        public static void Particleinit(Shapetype shapeType, float glsize, int bsteps, int mparticles, int changet, Color[] color, glowtype mygtype)
        {
            myShapetype = shapeType;
            glowsize = glsize;
            blursteps = bsteps;
            maxparticles = mparticles;
            changetime = changet;
            _color = new Color[color.Length];
            /*this._color =*/
            color.CopyTo(_color, 0);
            myglowtype = mygtype;
        }
        public Particle(float x, float y, float size, float speedX, float speedY, Color color, int shapeType )
        {
            this.x = x;
            this.y = y;
            this.size = size;
            this.speedX = speedX;
            this.speedY = speedY;
            this.color = color;
            this.shapeType = shapeType;
        }


        public void Update()
        {
            x += speedX;
            y += speedY;

            if (x < 0 || x > 800)
            {
                speedX = -speedX;
            }

            if (y < 0 || y > 600)
            {
                speedY = -speedY;
            }
        }

        public void Draw(Graphics graphics)
        {
            if (myglowtype == glowtype.Neon)
            {
                float gsize = glowsize; // Adjust the size of the glow
                int nBlurSteps = blursteps; // Number of blur layers

                for (int i = 0; i < nBlurSteps; i++)
                {
                    float blurRadius = gsize * (i + 1) / nBlurSteps;
                    int alpha = (int)(150 / nBlurSteps); // Adjust opacity based on number of blur steps

                    Color blurColor = Color.FromArgb(alpha, color); // Transparent color with reduced alpha

                    using (Brush blurBrush = new SolidBrush(blurColor))
                    {
                        if (myShapetype == Shapetype.circle || myShapetype == Shapetype.Mixed && shapeType < 5)
                        {
                            graphics.FillEllipse(blurBrush, x - size / 2 - blurRadius, y - size / 2 - blurRadius,
                                size + 2 * blurRadius, size + 2 * blurRadius);
                        }

                        if (myShapetype == Shapetype.Lines || myShapetype == Shapetype.Mixed && shapeType == 5)
                        {
                            graphics.DrawLine(new Pen(blurBrush, blurRadius), x - size / 2, y, x + size / 2, y);
                        }

                        if (myShapetype == Shapetype.Triangle || myShapetype == Shapetype.Mixed && shapeType == 6)
                        {
                            PointF[] points = new PointF[3];
                            points[0] = new PointF(x, y - size / 2 - blurRadius);
                            points[1] = new PointF(x + size / 2 + blurRadius, y + size / 2 + blurRadius);
                            points[2] = new PointF(x - size / 2 - blurRadius, y + size / 2 + blurRadius);
                            graphics.FillPolygon(blurBrush, points);
                        }
                    }
                }
            }

            if (myglowtype == glowtype.Bubble)
            {
                float gsize = glowsize; // Adjust the width of the glow
                Color glowColor = Color.FromArgb(100, Color.White); // White glow with 100 alpha (semi-transparent)

                using (Pen glowPen = new Pen(glowColor, gsize))
                {
                    if (myShapetype == Shapetype.circle || myShapetype == Shapetype.Mixed && shapeType <5)
                    {
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        graphics.DrawEllipse(glowPen, x - size / 2 - gsize, y - size / 2 - gsize, size + 2 * gsize, size + 2 * gsize);
                    }
                    else if (myShapetype == Shapetype.Lines || myShapetype == Shapetype.Mixed && shapeType == 5)
                    {
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        graphics.DrawLine(glowPen, x - size / 2, y, x + size / 2, y);
                    }
                    else if (myShapetype == Shapetype.Triangle || myShapetype == Shapetype.Mixed && shapeType == 6)
                    {
                        PointF[] points = new PointF[3];
                        points[0] = new PointF(x, y - size / 2 - gsize);
                        points[1] = new PointF(x + size / 2 + gsize, y + size / 2 + gsize);
                        points[2] = new PointF(x - size / 2 - gsize, y + size / 2 + gsize);
                        graphics.DrawPolygon(glowPen, points);
                    }
                }
            }

            if (myglowtype == glowtype.Neon_Bubble)
            {
                float gsize = glowsize; // Adjust the size of the glow
                Color glowColor = Color.FromArgb(100, color); // Colorful glow with reduced alpha (semi-transparent)

                using (Brush glowBrush = new SolidBrush(glowColor))
                {
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    RectangleF glowRect = new RectangleF(x - size / 2 - gsize, y - size / 2 - gsize, size + 2 * gsize,
                        size + 2 * gsize);
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddEllipse(glowRect);

                        // Create a radial gradient brush for the glow
                        PathGradientBrush gradientBrush = new PathGradientBrush(path);
                        gradientBrush.CenterPoint = new PointF(x, y);
                        gradientBrush.CenterColor = Color.Transparent;
                        gradientBrush.SurroundColors = new Color[] { glowColor };

                        graphics.FillPath(gradientBrush, path);
                    }
                }
            }

            if (myglowtype == glowtype.Shadow)
            {
                float gsize = glowsize; // Adjust the size of the glow
                int nBlurSteps = blursteps; // Number of steps for the blur effect

                for (int i = 0; i < nBlurSteps; i++)
                {
                    float blurRadius = gsize * (i + 1) / nBlurSteps;

                    using (Brush glowBrush =
                           new SolidBrush(Color.FromArgb(10, color))) // Adjust opacity (20) for the glow color
                    {
                        if (myShapetype == Shapetype.circle || myShapetype == Shapetype.Mixed && shapeType < 5)
                        {
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            graphics.FillEllipse(glowBrush, x - blurRadius / 2, y - blurRadius / 2, size + blurRadius,
                                size + blurRadius);
                        }

                        if (myShapetype == Shapetype.Triangle || myShapetype == Shapetype.Mixed && shapeType == 6)
                        {
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            PointF[] points = new PointF[3];
                            points[0] = new PointF(x, y - size / 2);
                            points[1] = new PointF(x + size / 2, y + size / 2);
                            points[2] = new PointF(x - size / 2, y + size / 2);
                            graphics.FillPolygon(glowBrush, points); //triangle
                        }

                        if (myShapetype == Shapetype.Lines || myShapetype == Shapetype.Mixed && shapeType == 5)
                        {
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            graphics.DrawLine(new Pen(glowBrush), x - size / 2, y, x + size / 2, y); //lines
                        }
                    }
                }
            }

            using (Brush brush = new SolidBrush(color))
            {
                if (myShapetype == Shapetype.Triangle || myShapetype == Shapetype.Mixed && shapeType == 6)
                {
                    PointF[] points = new PointF[3];
                    points[0] = new PointF(x, y - size / 2);
                    points[1] = new PointF(x + size / 2, y + size / 2);
                    points[2] = new PointF(x - size / 2, y + size / 2);
                    graphics.FillPolygon(brush, points);
                }

                if (myShapetype == Shapetype.Lines || myShapetype == Shapetype.Mixed && shapeType == 5)
                {
                    graphics.DrawLine(new Pen(brush), x - size / 2, y, x + size / 2, y);
                }

                if (myShapetype == Shapetype.circle || myShapetype == Shapetype.Mixed && shapeType < 5 )
                {
                    graphics.FillEllipse(brush, x - size / 2, y - size / 2, size, size);
                }
            }

        }

        public void ChangeColor(Color newColor)
        {
            color = newColor;
        }
    }
    public class ParticleSystem
    {
        public List<Particle> particles;
        private Timer timer;
        private Random random;
        public Color[] colors;
        private int currentColorIndex;
        //private Particle pt;
        private int formWidth;
        private int formHeight;
        public ParticleSystem(Particle.Shapetype shapeType, float glowsize, int blursteps, int maxparticles, int changetime, Color[] color, Particle.glowtype myglowtype, int formWidth, int formHeight)
        {
            particles = new List<Particle>();
            timer = new Timer();
            timer.Interval = 20;
            timer.Tick += Timer_Tick;
            //DoubleBuffered = true;
            random = new Random();
            //colors = new Color[] { Color.Yellow, Color.Red, Color.Blue, Color.Green, Color.White, Color.Pink };
            this.formWidth = formWidth;
            this.formHeight = formHeight;
            currentColorIndex = 0;
            this.colors = color;
            //color.CopyTo(this.colors, 0);
            Particle.Particleinit(shapeType, glowsize, blursteps, maxparticles, changetime, this.colors, myglowtype);
        }
        public static int mode = 0;
        #region Particles
        public void Start()
        {
            timer.Start();
            InitializeParticles();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateParticles();
            // Trigger redraw event
            OnParticlesUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateParticles()
        {
            foreach (Particle particle in particles)
            {
                particle.Update();
            }

            // Change color every 7 seconds
            if (DateTime.Now.Second % Particle.changetime == 0)
            {
                currentColorIndex = (currentColorIndex + 1) % colors.Length;
                foreach (Particle particle in particles)
                {
                    particle.ChangeColor(colors[currentColorIndex]);
                }
            }
        }

        private void InitializeParticles()
        {
            particles.Clear(); // Clear existing particles
            int numParticles = Particle.maxparticles; // Number of particles to create

            for (int i = 0; i < numParticles; i++)
            {
                float x = random.Next(formWidth);
                float y = random.Next(formHeight);
                float size = random.Next(5, 20);
                float speedX = (float)(random.NextDouble() * 4 - 2);
                float speedY = (float)(random.NextDouble() * 4 - 2);
                int shapeType = random.Next(7); // Random shape type: 0-4: Circles, 5: Small Triangles, 6: Lines
                Particle particle = new Particle(x, y, size, speedX, speedY, colors[currentColorIndex], shapeType);
                particles.Add(particle);
            }
        }

        public event EventHandler OnParticlesUpdated; // Event to notify particle updates
        #endregion

        public static Color CustomBackground(int a, int r, int g, int b)
        {
            return Color.FromArgb(a, r, g, b);
        }

        public static Color SetDarkBackground(int index)
        {
            Color[] backgroundColors = {
                Color.Black,
                Color.DarkSlateGray,
                Color.MidnightBlue,
                Color.FromArgb(30,30,30)
            };
            return backgroundColors[index];
        }
    }
}
