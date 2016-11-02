﻿using System.Collections.Generic;
using SpaceSim.Controllers;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    interface ISpaceCraft : IGravitationalBody
    {
        string CraftName { get; }
        string CraftDirectory { get; }

        ISpaceCraft Parent { get; }
        List<ISpaceCraft> Children { get; }

        double Width { get; }
        double Height { get; }

        double TotalWidth { get; }
        double TotalHeight { get; }

        double Throttle { get; }
        double Thrust { get; }

        double DryMass { get; }
        double PropellantMass { get; }

        IEngine[] Engines { get; }
        IController Controller { get; }

        string CommandFileName { get; }

        bool OnGround { get; }

        void InitializeController(EventManager eventManager);

        void Stage();
        void DeployFairing();

        double GetDownrangeDistance(DVector2 pointOfReference);

        void SetThrottle(double throttle, int[] engineIds = null);

        void SetPitch(double pitch);
        void OffsetPitch(double offset);

        void OffsetRoll(double offset);
        void SetRoll(double roll);

        void OffsetYaw(double offset);
        void SetYaw(double yaw);

        void OffsetRelativePitch(double offset);
        void SetRelativePitch(double pitch);

        void SetParent(ISpaceCraft craft);
        void AddChild(ISpaceCraft child);
        void RemoveChild(ISpaceCraft child);

        void UpdateAnimations(TimeStep timeStep);
        void UpdateChildren(DVector2 position, DVector2 velocity);
    }
}
