using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;
using SpaceSim.Drawing;

namespace SpaceSim.Engines
{
    class RaptorSL2019 : EngineBase
    {
        public RaptorSL2019(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(30, 209, 173, 199), 50, 2, 0.2, 0.6, 0.15))
        {
        }

        // 200t thrust
        public override double Thrust(double ispMultiplier)
        {
            return (1822000.0 + 139000.0 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of ISP = F/m*g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return 563 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new RaptorSL2019(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "Raptor SL 200";
        }
    }
}
