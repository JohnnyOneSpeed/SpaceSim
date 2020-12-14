using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class RaptorSL430 : EngineBase
    {
        public RaptorSL430(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(30, 209, 173, 199), 50, 2, 0.2, 0.6, 0.15))
        {
        }

        // 300t thrust = 2942 kN
        public override double Thrust(double ispMultiplier)
        {
            return (2694000.0 + 247200.0 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of Isp = F / ṁ * g0
        // ṁ = F / Isp * g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return 845 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new RaptorSL430(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "Raptor SL 300";
        }
    }
}

