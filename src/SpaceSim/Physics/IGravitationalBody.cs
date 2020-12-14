using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Physics
{
    interface IGravitationalBody : IPhysicsBody
    {
        IMassiveBody GravitationalParent { get; }

        DVector2 AccelerationG { get; }

        double Apoapsis { get; }
        double Periapsis { get; }

        bool InOrbit { get; }

        void ResetAccelerations();
        void ResetOrientation();

        void ResolveGravitation(IPhysicsBody other);

        double GetRelativePitch();
        double GetRelativeAltitude();

        DVector2 GetRelativeVelocity();
        DVector2 GetInertialVelocity();
        DVector2 GetRelativeAcceleration();
        DVector2 GetInertialAcceleration();
        DVector2 GetLateralAcceleration();
        DVector2 GetLateralVelocity();
        DVector2 GetLateralPosition();

        void SetGravitationalParent(IMassiveBody parent);

        void FixedUpdate(TimeStep timeStep);
    }
}
