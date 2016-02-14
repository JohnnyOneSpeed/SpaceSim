﻿using SpaceSim.Drawing;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Merlin1D : EngineBase
    {
        public Merlin1D(int id, ISpaceCraft parent, DVector2 offset)
            : base(id, parent, offset, new EngineFlame(id, 100, 1, 0.2))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (756222.222 + 68888.889 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate()
        {
            return 274 * Throttle * 0.01;
        }
    }
}
