using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class F1 : EngineBase
    {
        public F1(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(95, 255, 255, 191), 100, 2, 0.2, 0.8, 0.2))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (6770000 + 1000000 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of Isp = F/ṁ*g0 => ṁ = F/Isp*g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return (2624.9 - 18.6 * ispMultiplier) * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new Merlin1D(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "F1";
        }
    }
}
