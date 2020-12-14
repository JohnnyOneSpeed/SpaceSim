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
    sealed class LM : SpaceCraftBase
    {
        public override string CraftName { get { return "Lunar Module"; } }
        public override string CommandFileName { get { return "LM.xml"; } }

        public override double DryMass { get { return 4280; } }

        public override double Width { get { return 5.0; } }
        public override double Height { get { return 7.04; } }

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

        public LM(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 10920)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Saturn/LM.png")
        {
            StageOffset = new DVector2(0, 10);

            Engines = new IEngine[1];
            var offset = new DVector2(0.0, Height * 0.48);
            Engines[0] = new DPS(0, this, offset);
        }

        //public override void RenderGdi(Graphics graphics, Camera camera)
        //{
        //    base.RenderGdi(graphics, camera);

        //    if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
        //    {
        //        string filename = MissionName + ".csv";

        //        if (!File.Exists(filename))
        //        {
        //            File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Throttle\r\n");
        //        }

        //        timestamp = DateTime.Now;

        //        string contents = string.Format("{0}, {1}, {2}, {3}\r\n",
        //            this.GetRelativeVelocity().Length() / 10,
        //            this.GetRelativeAcceleration().Length() * 100,
        //            this.GetRelativeAltitude() / 100,
        //            this.Throttle * 10);
        //        File.AppendAllText(filename, contents);
        //    }
        //}
    }
}
