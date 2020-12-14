using SpaceSim.Spacecrafts;
using VectorMath;

using System;
using SpaceSim.Common.Contracts.Commands;

namespace SpaceSim.Commands
{
    class ProgradeCommand : CommandBase
    {
        private double _adjustmentTime;
        private double _curentOrientation;

        public ProgradeCommand(Prograde prograde)
            : base(prograde.StartTime, prograde.Duration)
        {
            _adjustmentTime = prograde.InitialAdjustmentTime;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            // EventManager.AddMessage("Maneuvering to prograde", spaceCraft);

            _curentOrientation = spaceCraft.Pitch;
        }

        // Nothing to finalize
        public override void Finalize(SpaceCraftBase spaceCraft) { }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double altitude = spaceCraft.GetRelativeAltitude();
            double atmosphereheight = spaceCraft.GravitationalParent.AtmosphereHeight;

            DVector2 prograde = spaceCraft.GetRelativeVelocity();
            if (altitude > atmosphereheight)
               prograde = spaceCraft.GetInertialVelocity();

            prograde.Normalize();

            double retrogradeAngle = prograde.Angle();
            if (retrogradeAngle > 0)
            {
                retrogradeAngle -= Math.PI * 2;
            }

            double adjustRatio = (elapsedTime - StartTime) / _adjustmentTime;

            if (adjustRatio > 1)
            {
                spaceCraft.SetPitch(retrogradeAngle);
            }
            else
            {
                double interpolatedAdjust = MathHelper.LerpAngle(_curentOrientation, retrogradeAngle, adjustRatio);

                spaceCraft.SetPitch(interpolatedAdjust);
            }
        }
    }
}
