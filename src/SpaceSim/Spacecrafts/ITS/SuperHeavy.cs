using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using System.IO;
using SpaceSim.Common;
using SpaceSim.Spacecrafts.FalconCommon;

namespace SpaceSim.Spacecrafts.ITS
{
    class SuperHeavy : SpaceCraftBase
    {
        public override string CraftName { get { return "Super Heavy"; } }
        public override string CommandFileName { get { return "SuperHeavy.xml"; } }

        public override double DryMass { get { return _dryMass; } }
        public override double Width { get { return 9; } }
        public override double Height { get { return 70; } }

        public override double LiftingSurfaceArea { get { return Width * Height; } }
        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        DateTime timestamp = DateTime.Now;
        double _dryMass;

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.8);
                double alpha = GetAlpha();
                return baseCd * Math.Sin(alpha * 2);
            }
        }

        public override double FrontalArea
        {
            get
            {
                double area = Math.PI * Math.Pow(Width / 2, 2);
                double alpha = GetAlpha();
                return Math.Abs(area * Math.Cos(alpha));
            }
        }

        public override double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2) * Height + Math.PI * Math.Pow(Width / 2, 2);
            }
        }

        public override Color IconColor
        {
            get { return Color.White; }
        }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.4);
                bool isRetrograde = false;

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    if (_gridFins[0].Pitch > 0)
                    {
                        baseCd = GetBaseCd(0.8);
                    }
                    else
                    {
                        baseCd = GetBaseCd(0.6);
                    }

                    isRetrograde = true;
                }

                double dragCoefficient = Math.Abs(baseCd * Math.Cos(alpha));
                double dragPreservation = 1.0;

                if (isRetrograde)
                {
                    // if retrograde
                    if (Throttle > 0 && MachNumber > 1.5 && MachNumber < 20.0)
                    {
                        double throttleFactor = Throttle / 50;
                        double cantFactor = Math.Sin(Engines[0].Cant * 2);
                        dragPreservation += throttleFactor * cantFactor;
                        dragCoefficient *= dragPreservation;
                    }
                }

                return Math.Abs(dragCoefficient);
            }
        }

        private SSGridFin[] _gridFins;
        private double _sootRatio = 0.0;

        //private SpriteSheet _spriteSheet;

        public SuperHeavy(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 3300000, double dryMass = 230000, int engineCount = 28, int thrust1 = 210, int thrust2 = 300)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Its/SuperHeavy.png")
        {
            StageOffset = new DVector2(0, 59.0);

            double finOffset = -27.0;
            _gridFins = new[]
            {
                new SSGridFin(this, new DVector2(4.0, finOffset), true),
                new SSGridFin(this, new DVector2(-4.0, finOffset), false)
            };

            Engines = new IEngine[engineCount];
            int nGimbal = 7, nFixed = 24;
            switch (engineCount)
            {
                case 28:
                    nGimbal = 8;
                    nFixed = 20;
                    break;
            }

            for (int i = 0; i < nGimbal; i++)
            {
                double theta = Math.PI / 2.0 + (Math.PI * i) / 3.5;
                double engineOffsetX = Math.Cos(theta);
                var offset = new DVector2(engineOffsetX * Width * 0.2, Height * 0.4);
                switch (thrust1)
                {
                    case 210:
                        Engines[i] = new RaptorSL300(i, this, offset);
                        break;
                    case 250:
                        Engines[i] = new RaptorSL360(i, this, offset);
                        break;
                    case 300:
                        Engines[i] = new RaptorSL430(i, this, offset);
                        break;
                    default:
                        Engines[i] = new RaptorSL2019(i, this, offset);
                        break;
                }
            }

            if (nFixed == 20)
            {
                for (int i = 0; i < nFixed; i++)
                {
                    double theta = Math.PI / 2.0 + (Math.PI * i) / 6.0;
                    double engineOffsetX = Math.Cos(theta);
                    var offset = new DVector2(engineOffsetX * Width * 0.35, Height * 0.4);

                    Engines[i + 8] = new RaptorSL430(i + 8, this, offset);
                }
            }
            else
            {
                if (engineCount >= 13)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        double theta = Math.PI / 2.0 + (Math.PI * i) / 10.0;
                        double engineOffsetX = Math.Cos(theta);
                        var offset = new DVector2(engineOffsetX * Width * 0.3, Height * 0.4);

                        switch (thrust2)
                        {
                            case 250:
                                Engines[i + 7] = new RaptorSL360(i + 7, this, offset);
                                break;
                            case 300:
                                Engines[i + 7] = new RaptorSL430(i + 7, this, offset);
                                break;
                            default:
                                Engines[i + 7] = new RaptorSL2019(i + 7, this, offset);
                                break;
                        }
                    }
                }

                if (engineCount >= 19)
                {
                    for (int i = 6; i < 12; i++)
                    {
                        double theta = Math.PI / 2.0 + (Math.PI * i) / 6.0;
                        double engineOffsetX = Math.Cos(theta);
                        var offset = new DVector2(engineOffsetX * Width * 0.3, Height * 0.4);

                        switch (thrust2)
                        {
                            case 250:
                                Engines[i + 7] = new RaptorSL360(i + 7, this, offset);
                                break;
                            case 300:
                                Engines[i + 7] = new RaptorSL430(i + 7, this, offset);
                                break;
                            default:
                                Engines[i + 7] = new RaptorSL2019(i + 7, this, offset);
                                break;
                        }
                    }
                }

                if (engineCount >= 25)
                {
                    if (engineCount >= 31)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            double theta = Math.PI / 2.0 + (Math.PI * i) / 6.0;
                            double engineOffsetX = Math.Cos(theta);
                            var offset = new DVector2(engineOffsetX * Width * 0.35, Height * 0.4);

                            switch (thrust2)
                            {
                                case 250:
                                    Engines[i + 19] = new RaptorSL360(i + 19, this, offset);
                                    break;
                                case 300:
                                    Engines[i + 19] = new RaptorSL430(i + 19, this, offset);
                                    break;
                                default:
                                    Engines[i + 19] = new RaptorSL2019(i + 19, this, offset);
                                    break;
                            }
                        }

                        if (engineCount >= 37)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                double theta = Math.PI / 2.0 + (Math.PI * i) / 3.0;
                                double engineOffsetX = Math.Cos(theta);
                                var offset = new DVector2(engineOffsetX * Width * 0.4, Height * 0.4);

                                switch (thrust2)
                                {
                                    case 250:
                                        Engines[i + 31] = new RaptorSL360(i + 31, this, offset);
                                        break;
                                    case 300:
                                        Engines[i + 31] = new RaptorSL430(i + 31, this, offset);
                                        break;
                                    default:
                                        Engines[i + 31] = new RaptorSL2019(i + 31, this, offset);
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            double theta = Math.PI / 2.0 + (Math.PI * i) / 3.0;
                            double engineOffsetX = Math.Cos(theta);
                            var offset = new DVector2(engineOffsetX * Width * 0.35, Height * 0.4);

                            switch (thrust1)
                            {
                                case 250:
                                    Engines[i + 19] = new RaptorSL360(i + 19, this, offset);
                                    break;
                                case 300:
                                    Engines[i + 19] = new RaptorSL430(i + 19, this, offset);
                                    break;
                                default:
                                    Engines[i + 19] = new RaptorSL2019(i + 19, this, offset);
                                    break;
                            }
                        }
                    }
                }
            }

            //_spriteSheet = new SpriteSheet("Textures/Spacecraft/Its/booster.png", 4, 12);

            string texturePath = "Its/SuperHeavy.png";
            string fullPath = Path.Combine("Textures/Spacecrafts", texturePath);
            this.Texture = new Bitmap(fullPath);
            _dryMass = dryMass;
        }

        public override void DeployGridFins()
        {
            foreach (SSGridFin gridFin in _gridFins)
            {
                gridFin.Deploy();
            }
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            foreach (SSGridFin gridFin in _gridFins)
            {
                gridFin.Update(dt);
                gridFin.UpdateSootRatio(_sootRatio);
            }

            //foreach (Fin fin in Fins)
            //{
            //    fin.Update(dt);
            //}
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            // Normalize the angle to [0,360]
            int rollAngle = (int)(Roll * MathHelper.RadiansToDegrees) % 360;
            int heatingRate = Math.Min((int)this.HeatingRate, 1000000);

            double AoA = GetAlpha();
            if (AoA > Constants.PiOverTwo || AoA < -Constants.PiOverTwo)
            {
                if (heatingRate > 4000)
                {
                    Random rnd = new Random();
                    float noise = (float)rnd.NextDouble();

                    // vary the bow shock width with velocity
                    double mach = this.MachNumber;
                    double theta = Math.PI / (4 * mach);
                    float width = screenBounds.Width * (float)Math.Sin(theta) * (10 + noise);
                    float height = screenBounds.Height / (2 + noise / 40);
                    RectangleF plasmaRect = screenBounds;
                    plasmaRect.Inflate(new SizeF(width, height));

                    if (rollAngle <= 90)
                    {
                        plasmaRect.Offset(0.0f, -screenBounds.Height / 2.4f);
                    }
                    else
                    {
                        plasmaRect.Offset(-screenBounds.Width / 2.0f, -screenBounds.Height / 2.0f);
                    }

                    int alpha = Math.Min(heatingRate / 1000, 255);
                    int red = alpha;
                    int green = 0;
                    int blue = alpha;
                    Color glow = Color.FromArgb(alpha, red, green, blue);

                    float penWidth = width / 100;
                    Pen glowPen = new Pen(glow, penWidth);
                    glowPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    glowPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                    //graphics.DrawEllipse(glowPen, plasmaRect);

                    if (rollAngle <= 90)
                    {
                        float startAngle = 60;
                        float sweepAngle = 60;
                        int arcs = 15;
                        for (int i = 0; i < arcs; i++)
                        {
                            glow = Color.FromArgb(alpha, (int)(red * (arcs - i) / (arcs * 1.3)), green, blue);
                            glowPen.Color = Color.FromArgb((int)(alpha * (arcs - i) / arcs), glow);
                            plasmaRect.Inflate(-penWidth, -penWidth);
                            graphics.DrawArc(glowPen, plasmaRect, startAngle - i * 4.0f, sweepAngle + i * 8.0f);
                        }

                        //for (int i = 0; i < arcs; i++)
                        //{
                        //    glow = Color.FromArgb(alpha, (int)(red * (arcs - i) / (arcs * 1.3)), green, blue);
                        //    glowPen.Color = Color.FromArgb((int)(alpha * (arcs - i) / arcs), glow);
                        //    plasmaRect.Inflate(-penWidth, -penWidth);
                        //    graphics.DrawArc(glowPen, plasmaRect, startAngle - i * 4.0f, sweepAngle + i * 8.0f);
                        //}
                    }
                    //else
                    //{
                    //    float startAngle = 175;
                    //    float sweepAngle = 30;
                    //    int arcs = 15;
                    //    for (int i = 0; i < arcs; i++)
                    //    {
                    //        glow = Color.FromArgb(alpha, (int)(red * (arcs - i) / (arcs * 1.3)), green, blue);
                    //        glowPen.Color = Color.FromArgb((int)(alpha * (arcs - i) / arcs), glow);
                    //        plasmaRect.Inflate(-penWidth, -penWidth);
                    //        try
                    //        {
                    //            graphics.DrawArc(glowPen, plasmaRect, startAngle + i * 1, sweepAngle + i * 6);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            string message = ex.Message;
                    //        }
                    //    }
                    //}
                }
            }

            if (rollAngle <= 90)
                graphics.DrawImage(this.Texture, screenBounds.X - screenBounds.Width * 0.05f, screenBounds.Y, screenBounds.Width * 1.1f, screenBounds.Height);
            // graphics.DrawImage(this.Texture, screenBounds.X - screenBounds.Width * 0.35f, screenBounds.Y, screenBounds.Width * 1.7f, screenBounds.Height);
            else
                graphics.DrawImage(this.Texture, screenBounds.X + screenBounds.Width * 0.35f, screenBounds.Y, -screenBounds.Width * 1.7f, screenBounds.Height);
            //graphics.DrawImage(this.Texture, screenBounds.X + screenBounds.Width * 0.35f, screenBounds.Y, -screenBounds.Width * 1.7f, screenBounds.Height);


            graphics.ResetTransform();

            foreach (SSGridFin gridFin in _gridFins)
            {
                gridFin.RenderGdi(graphics, camera);
            }
        }
    }
}

