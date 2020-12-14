using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class J2 : EngineBase
    {
        public J2(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 221, 192, 220), 100, 2, 1.0, 1.2, 0.16))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (486200.0 + 546900.0 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of Isp = F/ṁ*g0 => ṁ = F/Isp*g0 Isp = 465.5
        public override double MassFlowRate(double ispMultiplier)
        {
            return (247.9 + 2.3 * ispMultiplier) * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new J2(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "J-2";
        }
    }
}
