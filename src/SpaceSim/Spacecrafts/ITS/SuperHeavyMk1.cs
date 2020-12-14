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
    class SuperHeavyMk1 : SpaceCraftBase
    {
        public override string CraftName { get { return "Super Heavy"; } }
        public override string CommandFileName { get { return "SuperHeavy.xml"; } }

        public override double DryMass { get { return 130000; } }
        public override double Width { get { return 9; } }
        public override double Height { get { return 68; } }

        public override double LiftingSurfaceArea { get { return Width * Height; } }
        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }
        DateTime timestamp = DateTime.Now;

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
                    //if (_gridFins[0].Pitch > 0)
                    //{
                    //    baseCd = GetBaseCd(0.8);
                    //}
                    //else
                    //{
                        baseCd = GetBaseCd(0.6);
                    //}

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

        //private GridFin[] _gridFins;

        //private SpriteSheet _spriteSheet;

        public SuperHeavyMk1(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 4800000)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Its/SuperHeavy.png")
        {
            StageOffset = new DVector2(0, 54);

            Fins = new Fin[2];
            Fins[0] = new Fin(this, new DVector2(-1.2, -18.0), new DVector2(2.5, 5), 0, "Textures/Spacecrafts/ITS/Canard2.png", 1.5);
            Fins[1] = new Fin(this, new DVector2(2.0, 24.0), new DVector2(5.86, 17.0), -Math.PI / 6, "Textures/Spacecrafts/ITS/Fin.png", 3);

            Engines = new IEngine[19];

            for (int i = 0; i < 7; i++)
            {
                double theta = (Math.PI * i) / 7.0;
                double engineOffsetX = Math.Cos(theta);

                var offset = new DVector2(engineOffsetX * Width * 0.1, Height * 0.4);

                Engines[i] = new RaptorSL300(i, this, offset);
            }

            for (int i = 0; i < 12; i++)
            {
                double theta = (Math.PI * i) / 12.0;
                double engineOffsetX = Math.Cos(theta);

                var offset = new DVector2(engineOffsetX * Width * 0.3, Height * 0.4);

                Engines[i + 7] = new RaptorSL300(i + 7, this, offset);
            }

            //_spriteSheet = new SpriteSheet("Textures/Spacecraft/Its/booster.png", 4, 12);

            string texturePath = "Its/SuperHeavy.png";
            string fullPath = Path.Combine("Textures/Spacecrafts", texturePath);
            this.Texture = new Bitmap(fullPath);
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            foreach (Fin fin in Fins)
            {
                fin.Update(dt);
            }
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            graphics.DrawImage(this.Texture, screenBounds.X - screenBounds.Width * 0.6f, screenBounds.Y, screenBounds.Width * 2.1f, screenBounds.Height);
            graphics.ResetTransform();

            foreach (Fin fin in Fins)
            {
                fin.RenderGdi(graphics, camera);
            }
        }
    }
}

