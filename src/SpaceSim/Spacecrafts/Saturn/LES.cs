using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.Saturn
{
    class LES : SpaceCraftBase
    {
        public override string CraftName { get { return "LES"; } }
        public override string CommandFileName { get { return "LES.xml"; } }

        public override double Width { get { return 1.75; } }
        public override double Height { get { return 10.05; } }
        public override double DryMass { get { return 2581; } } // Apollo 8
        // public override double DryMass { get { return 2173; } } // Apollo 11

        public override AeroDynamicProperties GetAeroDynamicProperties
        {
            get { return AeroDynamicProperties.ExposedToAirFlow; }
        }

        public override double StagingForce { get { return 1500; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.4);
                double alpha = GetAlpha();
                return baseCd * Math.Cos(alpha);
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();
                return baseCd * Math.Sin(alpha * 2.0);
            }
        }

        public override double FrontalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }
        public override double ExposedSurfaceArea { get { return 2 * Math.PI * (Width / 2) * Height + FrontalArea; } }
        public override double LiftingSurfaceArea { get { return Width * Height; } }
        public override Color IconColor { get { return Color.White; } }

        public LES(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 1459)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Saturn/LES.png", null)
        {
            StageOffset = new DVector2(0, -5.75);

            Engines = new IEngine[]
            {
                // N.B. two motors with half the thrust of the real thing
                new ApolloLaunchEscapeMotor(0, this, new DVector2(-0.2, -3.5), -0.5),
                new ApolloLaunchEscapeMotor(1, this, new DVector2(0.2, -3.5), 0.5)
            };
        }
    }
}