using SpaceSim.Common.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class YawCommand : CommandBase
    {
        private double _targetOrientation;
        private double _currentOrientation;
        private double _displayOrientation;

        public YawCommand(Yaw orient)
            : base(orient.StartTime, orient.Duration)
        {
            _targetOrientation = orient.TargetOrientation * MathHelper.DegreesToRadians;
            _displayOrientation = orient.TargetOrientation;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddMessage($"Yawing to {_displayOrientation.ToString("0.0")} degrees", spaceCraft);

            _currentOrientation = spaceCraft.Yaw;
        }

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.SetYaw(_targetOrientation);
        }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double ratio = (elapsedTime - StartTime) / Duration;

            spaceCraft.SetYaw(_currentOrientation * (1 - ratio) + _targetOrientation * ratio);
        }
    }
}
