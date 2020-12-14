using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class RaptorVac2019 : EngineBase
    {
        public RaptorVac2019(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 209, 173, 199), 200, 2, 0.2, 1.2, 0.15))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (1700000.0 + 457000.0 * ispMultiplier) * Throttle * 0.01;
        }

        // ṁ = F / Isp * g0
        public override double MassFlowRate(double ispMultiplier)
        {
            // return 631 * Throttle * 0.01;    // Isp 375
            return 578 * Throttle * 0.01;    // Isp 380
        }

        public override IEngine Clone()
        {
            return new RaptorVac(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "Raptor Vac";
        }
    }
}
