using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class DPS : EngineBase
    {
        private double _angle;

        public DPS(int id, ISpaceCraft parent, DVector2 offset, double angle = 0.0)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 255, 255, 159), 50, 2, 1.0, 1.2, 0.1, angle))
        {
            _angle = angle;
        }

        public override double Thrust(double ispMultiplier)
        {
            return 45040 * Throttle * 0.01;
        }

        // Based off of Isp = F/ṁ*g0 => ṁ = F/Isp*g0 Isp = 319
        public override double MassFlowRate(double ispMultiplier)
        {
            return 14.77 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new DPS(0, Parent, Offset, _angle);
        }

        public override string ToString()
        {
            return "DPS";
        }
    }
}
