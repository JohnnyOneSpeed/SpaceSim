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
    sealed class SIVB : SpaceCraftBase
    {
        public override string CraftName { get { return "S-IVB"; } }
        public override string CommandFileName { get { return "SIVB.xml"; } }

        public override double DryMass { get { return 13496; } } // Apollo 8
        //public override double DryMass { get { return 13290; } } // Apollo 11

        public override double Width { get { return 6.6; } }
        public override double Height { get { return 22.4; } }
        //public override double Height { get { return 10.1 + 16; } }


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

        public SIVB(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 106371) // Apollo 8
        // public SIVB(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 106830) // Apollo 11
            : base(craftDirectory, position, velocity, 0, propellantMass, "Saturn/SIVB.png")
        {
            StageOffset = new DVector2(0, 3.5);

            Engines = new IEngine[1];
            var offset = new DVector2(0.0, Height * 0.8);
            Engines[0] = new J2(0, this, offset);
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
