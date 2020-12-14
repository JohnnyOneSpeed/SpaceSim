using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;
using SpaceSim.Drawing;
using SpaceSim.Properties;
using System.IO;

namespace SpaceSim.Spacecrafts.Saturn
{
    sealed class SII: SpaceCraftBase
    {
        public override string CraftName { get { return "S-II"; } }
        public override string CommandFileName { get { return "SII.xml"; } }

        public override double DryMass { get { return 40188; } } // Apollo 8
        // public override double DryMass { get { return 34450; } } // Apollo 11

        public override double Width { get { return 10.1; } }
        public override double Height { get { return 20.6; } }
        //public override double Height { get { return 24.8 + 5.0; } }

        public override AeroDynamicProperties GetAeroDynamicProperties
        {
            get
            {
                return AeroDynamicProperties.ExtendsFineness;
            }
        }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.4);
                double dragCoefficient = Math.Abs(baseCd * Math.Cos(alpha));
                return Math.Abs(dragCoefficient);
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
                return 2 * Math.PI * (Width / 2) * Height + FrontalArea;
            }
        }

        public override double LiftingSurfaceArea
        {
            get
            {
                return Width * Height;
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();
                return baseCd * Math.Sin(alpha * 2);
            }
        }

        public override Color IconColor
        {
            get
            {
                return Color.White;
            }
        }

        DateTime timestamp = DateTime.Now;

        
        public SII(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 429490) // Apollo 8
        // public SII(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 444180) // Apollo 11
            : base(craftDirectory, position, velocity, 0, propellantMass, "Saturn/SII.png")
        {
            StageOffset = new DVector2(0, 22.5);

            Engines = new IEngine[5];

            for (int i = 0; i < 5; i++)
            {
                double theta = Math.PI / 2.0 + (Math.PI * i) / 2.5;
                double engineOffsetX = Math.Cos(theta);
                var offset = new DVector2(engineOffsetX * Width * 0.25, Height * 0.72);
                Engines[i] = new J2(i, this, offset);
            }
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f, screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            graphics.DrawImage(this.Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height * 1.33f);
            graphics.ResetTransform();
        }
    }
}
