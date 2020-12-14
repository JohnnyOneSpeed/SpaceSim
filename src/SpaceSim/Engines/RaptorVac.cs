﻿using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class RaptorVac : EngineBase
    {
        public RaptorVac(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(23, 209, 173, 199), 200, 2, 0.2, 1.2, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            //return 1900000.0 * Throttle * 0.01;
            return (1650000.0 + 250000.0 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate(double ispMultiplier)
        {
            return 525 * Throttle * 0.01;
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
