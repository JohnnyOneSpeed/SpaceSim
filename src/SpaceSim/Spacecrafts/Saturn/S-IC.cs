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
    sealed class SIC : SpaceCraftBase
    {
        public override string CraftName { get { return "S-IC"; } }
        public override string CommandFileName { get { return "SIC.xml"; } }

        public override double DryMass { get { return 138640; } } // Apollo 8
        // public override double DryMass { get { return 130980; } } // Apollo 11

        public override double Width { get { return 10.1; } }
        public override double Height { get { return 43.6; } }

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

        public SIC(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 2033574) // Apollo 8
        //public SIC(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 2147270) // Apollo 11
            : base(craftDirectory, position, velocity, 0, propellantMass, "Saturn/SIC.png")
        {
            StageOffset = new DVector2(0, 24);

            Engines = new IEngine[5];

            for (int i = 0; i < 5; i++)
            {
                double theta = Math.PI / 2.0 + (Math.PI * i) / 2.5;
                double engineOffsetX = Math.Cos(theta);
                var offset = new DVector2(engineOffsetX * Width * 0.4, Height * 0.48);
                Engines[i] = new F1(i, this, offset);
            }
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            graphics.DrawImage(Texture, screenBounds.X - screenBounds.Width * 0.30f, screenBounds.Y, screenBounds.Width * 1.6f, screenBounds.Height);

            graphics.ResetTransform();
        }
    }
}
