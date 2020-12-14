using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using SpaceSim.Drawing;

namespace SpaceSim.Spacecrafts.Saturn
{
    class Interstage : SpaceCraftBase
    {
        public override string CraftName { get { return "Interstage"; } }
        public override string CommandFileName { get { return "Interstage.xml"; } }

        public override double Width { get { return 9.8; } }
        public override double Height { get { return 6.25; } }

        public override double DryMass { get { return 1000; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.3);
                double alpha = GetAlpha();
                double cosAlpha = Math.Cos(alpha);
                double Cd = Math.Abs(baseCd * cosAlpha);

                return Cd;
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();
                double sinAlpha = Math.Sin(alpha * 2);
                return baseCd * sinAlpha;
            }
        }

        // Cylinder - 2 * pi * r * h
        public override double FrontalArea { get { return 27.6579; } }

        public override double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2) * Height + FrontalArea;
            }
        }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override Color IconColor { get { return Color.White; } }

        public Interstage(string craftDirectory, DVector2 position, DVector2 velocity, double yOffset = 12.0)
            : base(craftDirectory, position, velocity, 0, 0, "Saturn/Interstage.png")
        {
            StageOffset = new DVector2(0, yOffset);

            Engines = new IEngine[]{};
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f, screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            graphics.DrawImage(this.Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);
            graphics.ResetTransform();
        }
    }
}
