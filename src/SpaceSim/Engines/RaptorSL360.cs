using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class RaptorSL360 : EngineBase
    {
        public RaptorSL360(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(30, 209, 173, 199), 50, 2, 0.2, 0.6, 0.15))
        {
        }

        // 250t thrust
        public override double Thrust(double ispMultiplier)
        {
            return (2245000.0 + 206000.0 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of Isp = F / ṁ * g0
        // ṁ = F / Isp * g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return 694 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new RaptorSL360(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "Raptor SL 250";
        }
    }
}

