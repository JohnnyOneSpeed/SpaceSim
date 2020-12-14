﻿using System;
using System.Collections.Generic;
using SpaceSim.Common.Contracts;
using SpaceSim.Physics;
using SpaceSim.SolarSystem;
using SpaceSim.Spacecrafts.DeltaIV;
using SpaceSim.Spacecrafts.DragonV1;
using SpaceSim.Spacecrafts.DragonV2;
using SpaceSim.Spacecrafts.Falcon9;
using SpaceSim.Spacecrafts.Falcon9SSTO;
using SpaceSim.Spacecrafts.FalconCommon;
using SpaceSim.Spacecrafts.FalconHeavy;
using SpaceSim.Spacecrafts.ITS;
using SpaceSim.Spacecrafts.NewGlenn;
using VectorMath;

using SpaceSim.Spacecrafts.Electron;
using SpaceSim.Spacecrafts.SLS;
using SpaceSim.Spacecrafts.Saturn;

namespace SpaceSim.Spacecrafts
{
    static class SpacecraftFactory
    {
        public static List<ISpaceCraft> BuildSpaceCraft(IMassiveBody planet, double surfaceAngle, MissionConfig config, string craftDirectory)
        {
            if (string.IsNullOrEmpty(config.VehicleType))
            {
                throw new Exception("Must specify a vehicle type in the MissionConfig.xml!");
            }

            if (string.IsNullOrEmpty(config.ParentPlanet))
            {
                throw new Exception("Must specify a parent planet for the launch vehicle!");
            }

            var planetOffset = new DVector2(Math.Cos(surfaceAngle) * planet.SurfaceRadius,
                                           Math.Sin(surfaceAngle) * planet.SurfaceRadius);

            List<ISpaceCraft> spaceCrafts = GenerateSpaceCraft(planet, config, craftDirectory);

            foreach (ISpaceCraft craft in spaceCrafts)
            {
                craft.SkewEventTimes(config.TimeSkew);
            }

            if (spaceCrafts.Count == 0)
            {
                throw new Exception("No spacecrafts produced!");
            }

            ISpaceCraft primaryCraft = spaceCrafts[0];

            DVector2 distanceFromSurface = primaryCraft.Position - planet.Position;

            // If the ship is spawned on the planet update it's position to the correct surface angle
            if (distanceFromSurface.Length() * 0.999 < planet.SurfaceRadius)
            {
                primaryCraft.SetSurfacePosition(planet.Position + planetOffset, surfaceAngle);
            }
            else
            {
                primaryCraft.SetPitch(0);
            }

            return spaceCrafts;
        }

        private static List<ISpaceCraft> GenerateSpaceCraft(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            switch (config.VehicleType)
            {
                case "BFR Direct GTO":
                    return BuildBFRCrew(planet, config, craftDirectory);
                case "BFR Crew Launch":
                case "BFR300 Crew Launch":
                    return BuildBFR300Crew(planet, config, craftDirectory);
                case "BFR19 Crew Launch":
                    return BuildBFR19Crew(planet, config, craftDirectory);
                case "BFS to GEO":
                    return BuildBfsLeo(planet, config, craftDirectory);
                case "BFS250 to LEO":
                    return BuildBfs(planet, config, craftDirectory);
                case "BFS300 to LEO":
                    return BuildBfs300(planet, config, craftDirectory);
                case "BFS Earth EDL":
                    return BuildBfsEarthEdl(planet, config, craftDirectory);
                case "BFS Mars Return EDL":
                    return BuildBfsMarsReturnEdl(planet, config, craftDirectory);
                case "BFS Mars Return Skip Entry":
                    return BuildBfsMarsReturnSkipEntry(planet, config, craftDirectory);
                case "BFS Mars TEI":
                    return BuildBfsMarsTEI(planet, config, craftDirectory);
                case "Electron":
                    return BuildElectron(planet, config, craftDirectory);
                case "MiniBFS":
                    return BuildMiniBfs(planet, config, craftDirectory);
                case "GenericDH":
                    return BuildDeltaHeavy(planet, config, craftDirectory);
                case "GenericF9":
                    return BuildGenericF9(planet, config, craftDirectory);
                case "PolarF9":
                    return BuildPolarF9(planet, config, craftDirectory);
                case "GenericF9B5":
                    return BuildGenericF9B5(planet, config, craftDirectory);
                case "DragonF9":
                    return BuildF9Dragon(planet, config, craftDirectory);
                case "DragonF9B5":
                    return BuildF9B5Dragon(planet, config, craftDirectory);
                case "Dragon2F9":
                    return BuildF9Dragon2(planet, config, craftDirectory);
                case "X37B":
                    return BuildX37B(planet, config, craftDirectory);
                case "F9S2 Earth EDL":
                    return BuildF9S2EDL(planet, config, craftDirectory);
                case "F9S2 Earth LEO EDL":
                    return BuildF9S2LEOEDL(planet, config, craftDirectory);
                case "F9SSTO":
                    return BuildF9SSTO(planet, craftDirectory);
                case "DragonAbort":
                    return BuildDragonV2Abort(planet, config, craftDirectory);
                case "DragonEntry":
                    return BuildDragonV2Entry(planet, config, craftDirectory);
                case "DragonXL":
                    return BuildDragonXL(planet, config, craftDirectory);
                case "GreyDragonFH":
                    return BuildGreyDragonFH(planet, config, craftDirectory);
                case "GenericFH":
                    return BuildFalconHeavy(planet, config, craftDirectory);
                case "FH-B5":
                    return BuildFalconHeavyB5(planet, config, craftDirectory);
                case "FH-Demo":
                    return BuildFalconHeavyDemo(planet, config, craftDirectory);
                case "FH-Europa":
                    return BuildFalconHeavyEuropa(planet, config, craftDirectory);
                case "FH-PPE-HALO":
                    return BuildFalconHeavyPpeHalo(planet, config, craftDirectory);
                case "FH-PSP":
                    return BuildFalconHeavyPSP(planet, config, craftDirectory);
                case "FH-Orion":
                    return BuildFalconHeavyOrion(planet, config, craftDirectory);
                case "FH-Orion-ESM":
                    return BuildFalconHeavyOrionESM(planet, config, craftDirectory);
                case "FH-ICPS":
                    return BuildFalconHeavyICPS(planet, config, craftDirectory);
                case "FH-BOOSTER-ICPS":
                    return BuildFalconHeavyBoosterICPS(planet, config, craftDirectory);
                case "AutoLandingTest":
                    return BuildAutoLandingTest(planet, config, craftDirectory);
                case "ITS Crew Launch":
                    return BuildITSCrew(planet, config, craftDirectory);
                case "ITS Tanker SSTO":
                    return BuildITSTanker(planet, config, craftDirectory);
                case "ItsEDL":
                    return BuildItsEDL(planet, config, craftDirectory);
                case "New Glenn":
                    return BuildNewGlenn(planet, config, craftDirectory);
                case "OrionTLI":
                    return BuildOrionTLI(planet, config, craftDirectory);
                case "SaturnV":
                    return BuildSaturnV(planet, config, craftDirectory);
                case "Scaled BFR GTO":
                    return BuildScaledBfrGto(planet, config, craftDirectory);
                case "Scaled BFS TLI":
                    return BuildScaledBfsTLI(planet, config, craftDirectory);
                case "Scaled BFS LL":
                    return BuildScaledBfsLL(planet, config, craftDirectory);
                case "Scaled BFS TEI":
                    return BuildScaledBfsTEI(planet, config, craftDirectory);
                case "Scaled BFS EDL":
                    return BuildScaledBfsEDL(planet, config, craftDirectory);
                case "SLS Satellite":
                    return BuildSLS(planet, config, craftDirectory);
                case "SLS Orion":
                    return BuildSLSOrion(planet, config, craftDirectory);
                case "StarHopper":
                    return BuildStarHopper(planet, config, craftDirectory);
                case "StarKicker":
                    return BuildStarKicker(planet, config, craftDirectory);
                case "StarshipMk1":
                    return BuildStarshipMk1(planet, config, craftDirectory);
                case "StarshipP2P":
                    return BuildStarshipP2P(planet, config, craftDirectory);
                case "Starship2P2P":
                    return BuildStarship2P2P(planet, config, craftDirectory);
                case "StarshipSN4":
                    return BuildStarshipSN4(planet, config, craftDirectory);
                case "StarshipSN8":
                    return BuildStarshipSN8(planet, config, craftDirectory);
                case "StarshipTLI":
                    return BuildStarshipTLI(planet, config, craftDirectory);
                case "StarshipTMI":
                    return BuildStarshipTMI(planet, config, craftDirectory);
                case "Starship3SuperHeavy7":
                    return BuildStarship3SuperHeavy7(planet, config, craftDirectory);
                case "Starship3SuperHeavy13":
                    return BuildStarship3SuperHeavy13(planet, config, craftDirectory);
                case "Starship3SuperHeavy19":
                    return BuildStarship3SuperHeavy19(planet, config, craftDirectory);
                case "StarshipSuperHeavy25":
                    return BuildStarshipSuperHeavy25(planet, config, craftDirectory);
                case "StarshipSuperHeavy28":
                    return BuildStarshipSuperHeavy28(planet, config, craftDirectory);
                case "StarshipSuperHeavy31":
                    return BuildStarshipSuperHeavy31(planet, config, craftDirectory);
                case "StarshipSuperHeavy37":
                    return BuildStarshipSuperHeavy37(planet, config, craftDirectory);
                case "StarshipSuperHeavy360":
                    return BuildStarshipSuperHeavy360(planet, config, craftDirectory);
                case "StarshipSuperHeavy430":
                    return BuildStarshipSuperHeavy430(planet, config, craftDirectory);
                case "StarshipSuperHeavyMk1":
                    return BuildStarshipSuperHeavyMk1(planet, config, craftDirectory);
                default:
                    throw new Exception("Unknown vehicle type: " + config.VehicleType);
            }
        }

        private static List<ISpaceCraft> BuildElectron(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var kickStage = new ElectronKickStage(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                                      planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var leftFairing = new ElectronFairing(craftDirectory, kickStage.Position, DVector2.Zero, true);
            var rightFairing = new ElectronFairing(craftDirectory, kickStage.Position, DVector2.Zero, false);

            kickStage.AttachFairings(leftFairing, rightFairing);

            var leftBattery = new ElectronBattery(craftDirectory, kickStage.Position, DVector2.Zero, true);
            var rightBattery = new ElectronBattery(craftDirectory, kickStage.Position, DVector2.Zero, false);

            kickStage.AttachBatteries(leftBattery, rightBattery);

            var electronS1 = new ElectronS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var electronS2 = new ElectronS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            kickStage.AddChild(electronS2);
            electronS2.SetParent(kickStage);
            electronS2.AddChild(electronS1);
            electronS1.SetParent(electronS2);

            return new List<ISpaceCraft>
            {
                kickStage, electronS2, leftBattery, rightBattery, electronS1, leftFairing, rightFairing
            };
        }

        private static List<ISpaceCraft> BuildGenericF9(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset,
                planet.Velocity, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var f9S1 = new F9S1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 11.2);

            demoSat.AddChild(f9S2);
            f9S2.SetParent(demoSat);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                demoSat, f9S2, f9S1, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildPolarF9(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, 200000 - planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var f9S1 = new F9S1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 11.2);

            demoSat.AddChild(f9S2);
            f9S2.SetParent(demoSat);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                demoSat, f9S2, f9S1, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildGenericF9B5(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var f9S1 = new F9S1B5(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2B5(craftDirectory, DVector2.Zero, DVector2.Zero, 11.2);

            demoSat.AddChild(f9S2);
            f9S2.SetParent(demoSat);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                demoSat, f9S2, f9S1, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildX37B(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var x37b = new X37B(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, x37b.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, x37b.Position, DVector2.Zero, false);

            x37b.AttachFairings(fairingLeft, fairingRight);

            var f9S1 = new F9S1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 11.2);

            x37b.AddChild(f9S2);
            f9S2.SetParent(x37b);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                x37b, f9S2, f9S1, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildF9Dragon(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new Dragon(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);
            var dragonTrunk = new DragonTrunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var f9S1 = new F9S1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 8.3);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);
            dragonTrunk.AddChild(f9S2);
            f9S2.SetParent(dragonTrunk);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                dragon, dragonTrunk, f9S2, f9S1
            };
        }

        private static List<ISpaceCraft> BuildF9B5Dragon(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new Dragon(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);
            var dragonTrunk = new DragonTrunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var f9S1 = new F9S1B5(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2B5(craftDirectory, DVector2.Zero, DVector2.Zero, 8.3);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);
            dragonTrunk.AddChild(f9S2);
            f9S2.SetParent(dragonTrunk);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                dragon, dragonTrunk, f9S2, f9S1
            };
        }

        private static List<ISpaceCraft> BuildF9Dragon2(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new DragonV2.DragonV2(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass, 2563);
            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var f9S1 = new F9S1B5(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2B5(craftDirectory, DVector2.Zero, DVector2.Zero, 9.0);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);
            dragonTrunk.AddChild(f9S2);
            f9S2.SetParent(dragonTrunk);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                dragon, dragonTrunk, f9S2, f9S1
            };
        }

        private static List<ISpaceCraft> BuildF9S2LEOEDL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var s2 = new F9S2C(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 150000),
                                  planet.Velocity + new DVector2(-7000, 1000), 0, 1000);
            // planet.Velocity + new DVector2(-7400, 700), 0, 1000);

            return new List<ISpaceCraft>
            {
                s2
            };
        }

        private static List<ISpaceCraft> BuildF9S2EDL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var s2 = new F9S2B(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 150000),
                                  planet.Velocity + new DVector2(-10000, 850), 0, 1000);

            return new List<ISpaceCraft>
            {
                s2
            };
        }

        private static List<ISpaceCraft> BuildF9S2TJI(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 169500),
                planet.Velocity + new DVector2(-7795, 0), config.PayloadMass);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 8.3);

            demoSat.AddChild(f9S2);
            f9S2.SetParent(demoSat);

            return new List<ISpaceCraft>
            {
                demoSat, f9S2
            };
        }

        private static List<ISpaceCraft> BuildF9SSTO(IMassiveBody planet, string craftDirectory)
        {
            var f9SSTO = new F9SSTO(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity);

            return new List<ISpaceCraft> { f9SSTO };
        }

        private static List<ISpaceCraft> BuildDragonV2Abort(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new DragonV2.DragonV2(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity, config.PayloadMass, 446);
            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);

            return new List<ISpaceCraft> { dragon, dragonTrunk };
        }

        private static List<ISpaceCraft> BuildDragonV2Entry(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new GreyDragon.GreyDragon(craftDirectory, planet.Position + new DVector2(0, planet.SurfaceRadius + 200000.0),
                                               planet.Velocity + new DVector2(11000, -1650), config.PayloadMass, 446);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);

            dragon.SetPitch(Math.PI * 1.24);

            return new List<ISpaceCraft> { dragon, dragonTrunk };
        }

        private static List<ISpaceCraft> BuildDeltaHeavy(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new ParkerSolarProbe(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset,
                                      planet.Velocity + new DVector2(-400, 0) + config.VelocityOffset, config.PayloadMass);

            var fairingLeft = new DIVH5mFairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new DIVH5mFairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var dhS1 = new CommonBoosterCore(craftDirectory, DVector2.Zero, DVector2.Zero);
            var dhS2 = new DIVHS2(craftDirectory, DVector2.Zero, DVector2.Zero, 7.2);
            var dhS3 = new Star48PAM(craftDirectory, DVector2.Zero, DVector2.Zero, 3);

            var dhLeftBooster = new SideBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var dhRightBooster = new SideBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            demoSat.AddChild(dhS3);
            dhS3.SetParent(demoSat);
            dhS3.AddChild(dhS2);
            dhS2.SetParent(dhS3);
            dhS2.AddChild(dhS1);
            dhS1.SetParent(dhS2);
            dhS1.AddChild(dhLeftBooster);
            dhS1.AddChild(dhRightBooster);
            dhLeftBooster.SetParent(dhS1);
            dhRightBooster.SetParent(dhS1);

            return new List<ISpaceCraft>
            {
                demoSat, dhS3, dhS2, dhLeftBooster, dhS1, dhRightBooster, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildFalconHeavyPSP(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset,
                                      planet.Velocity + new DVector2(-400, 0) + config.VelocityOffset, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero, 11.3);
            var fhS3 = new Star48PAM(craftDirectory, DVector2.Zero, DVector2.Zero, 3);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            demoSat.AddChild(fhS3);
            fhS3.SetParent(demoSat);
            fhS3.AddChild(fhS2);
            fhS2.SetParent(fhS3);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                demoSat, fhS3, fhS2, fhLeftBooster, fhS1, fhRightBooster, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildFalconHeavy(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset,
                                      planet.Velocity + new DVector2(-400, 0) + config.VelocityOffset, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero, 11.3);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            demoSat.AddChild(fhS2);
            fhS2.SetParent(demoSat);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                demoSat, fhS2, fhLeftBooster, fhS1, fhRightBooster, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildFalconHeavyB5(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset,
                                      planet.Velocity + new DVector2(-400, 0) + config.VelocityOffset, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var fhS1 = new FHS1B5(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2B5(craftDirectory, DVector2.Zero, DVector2.Zero, 11.3);

            var fhLeftBooster = new FHBoosterB5(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBoosterB5(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            demoSat.AddChild(fhS2);
            fhS2.SetParent(demoSat);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                demoSat, fhS2, fhLeftBooster, fhS1, fhRightBooster, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildFalconHeavyEuropa(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ionSat = new IonDriveSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset,
                                      planet.Velocity + new DVector2(-400, 0) + config.VelocityOffset, config.PayloadMass, 900);

            var fairingLeft = new Fairing(craftDirectory, ionSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, ionSat.Position, DVector2.Zero, false);

            ionSat.AttachFairings(fairingLeft, fairingRight);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero, 11.3);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            ionSat.AddChild(fhS2);
            fhS2.SetParent(ionSat);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                ionSat, fhS2, fhLeftBooster, fhS1, fhRightBooster, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildFalconHeavyDemo(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var roadster = new Roadster(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius, 0) + config.PositionOffset,
                          planet.Velocity + new DVector2(0, -400) + config.VelocityOffset, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, roadster.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, roadster.Position, DVector2.Zero, false);

            roadster.AttachFairings(fairingLeft, fairingRight);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero, 8.4);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            roadster.AddChild(fhS2);
            fhS2.SetParent(roadster);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                roadster, fhS2, fhLeftBooster, fhS1, fhRightBooster, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildFalconHeavyOrion(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var orion = new Orion(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius, 0) + config.PositionOffset,
                          planet.Velocity + new DVector2(0, -400) + config.VelocityOffset, config.PayloadMass, 0);

            var las = new LAS(craftDirectory, orion.Position, DVector2.Zero);
            orion.AttachLAS(las);

            var esm = new ESM(craftDirectory, DVector2.Zero, DVector2.Zero);
            var icps = new ICPS(craftDirectory, DVector2.Zero, DVector2.Zero, 8.2);
            var fhS2 = new FHS2B5(craftDirectory, DVector2.Zero, DVector2.Zero, 13);
            var fhS1 = new FHS1B5(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhLeftBooster = new FHBoosterB5(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBoosterB5(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            orion.AddChild(esm);
            esm.SetParent(orion);
            esm.AddChild(icps);
            icps.SetParent(esm);
            icps.AddChild(fhS2);
            fhS2.SetParent(icps);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                orion, esm, icps, fhS2, fhLeftBooster, fhS1, fhRightBooster, las
            };
        }

        private static List<ISpaceCraft> BuildFalconHeavyOrionESM(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var orion = new Orion(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius, 0) + config.PositionOffset,
                          planet.Velocity + new DVector2(0, -400) + config.VelocityOffset, config.PayloadMass, 0);

            var las = new LAS(craftDirectory, orion.Position, DVector2.Zero);
            orion.AttachLAS(las);

            var esm = new ESM(craftDirectory, DVector2.Zero, DVector2.Zero);
            var interstage = new DragonV2.Interstage(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2B5(craftDirectory, DVector2.Zero, DVector2.Zero, 8.5);
            var fhS1 = new FHS1B5(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhLeftBooster = new FHBoosterB5(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBoosterB5(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            orion.AddChild(esm);
            esm.SetParent(orion);
            esm.AddChild(interstage);
            interstage.SetParent(esm);
            interstage.AddChild(fhS2);
            fhS2.SetParent(interstage);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                orion, esm, interstage, fhS2, fhLeftBooster, fhS1, fhRightBooster, las
            };
        }

        private static List<ISpaceCraft> BuildFalconHeavyICPS(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var icps = new ICPS(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius, 0) + config.PositionOffset,
                          planet.Velocity + new DVector2(0, -400) + config.VelocityOffset);

            var interstage = new DragonV2.Interstage(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2B5(craftDirectory, DVector2.Zero, DVector2.Zero, 8.5);
            var fhS1 = new FHS1B5(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhLeftBooster = new FHBoosterB5(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBoosterB5(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            icps.AddChild(interstage);
            interstage.SetParent(icps);
            interstage.AddChild(fhS2);
            fhS2.SetParent(interstage);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                icps, interstage, fhS2, fhLeftBooster, fhS1, fhRightBooster
            };
        }

        private static List<ISpaceCraft> BuildFalconHeavyBoosterICPS(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var icps = new ICPS(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius, 0) + config.PositionOffset,
                          planet.Velocity + new DVector2(0, -400) + config.VelocityOffset);

            var fairingLeft = new ICPSFairing(craftDirectory, icps.Position, DVector2.Zero, true);
            var fairingRight = new ICPSFairing(craftDirectory, icps.Position, DVector2.Zero, false);

            icps.AttachFairings(fairingLeft, fairingRight);

            var interstage = new DragonV2.Interstage(craftDirectory, DVector2.Zero, DVector2.Zero, 0.5);
            var fhS1 = new FHS1B5(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhLeftBooster = new FHBoosterB5(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBoosterB5(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            icps.AddChild(interstage);
            interstage.SetParent(icps);
            interstage.AddChild(fhS1);
            fhS1.SetParent(interstage);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                icps, interstage, fhLeftBooster, fhS1, fhRightBooster, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildOrionTLI(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var orion = new Orion(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius + 166000.0, 0) + config.PositionOffset,
                          planet.Velocity + new DVector2(0, -7817.1) + config.VelocityOffset, config.PayloadMass);

            var esm = new ESM(craftDirectory, DVector2.Zero, DVector2.Zero);
            var icps = new ICPS(craftDirectory, DVector2.Zero, DVector2.Zero, 8.2);

            orion.AddChild(esm);
            esm.SetParent(orion);
            esm.AddChild(icps);
            icps.SetParent(esm);

            return new List<ISpaceCraft>
            {
                orion, esm, icps
            };
        }

        private static List<ISpaceCraft> BuildRedDragonFH(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var redDragon = new RedDragon.RedDragon(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                                      planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            redDragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(redDragon);
            dragonTrunk.AddChild(fhS2);
            fhS2.SetParent(dragonTrunk);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                redDragon, dragonTrunk, fhS2, fhLeftBooster, fhS1, fhRightBooster
            };
        }

        private static List<ISpaceCraft> BuildDragonXL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new DragonV2.DragonXL(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity, config.PayloadMass, 1000);

            var fairingLeft = new Fairing(craftDirectory, dragon.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, dragon.Position, DVector2.Zero, false);

            dragon.AttachFairings(fairingLeft, fairingRight);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(fhS2);
            fhS2.SetParent(dragon);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                dragon, fhS2, fhLeftBooster, fhS1, fhRightBooster, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildGreyDragonFH(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new GreyDragon.GreyDragon(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity, config.PayloadMass, 446);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);
            dragonTrunk.AddChild(fhS2);
            fhS2.SetParent(dragonTrunk);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                dragon, dragonTrunk, fhS2, fhS1, fhLeftBooster, fhRightBooster
            };
        }

        private static List<ISpaceCraft> BuildFalconHeavyPpeHalo(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset,
                                      planet.Velocity + new DVector2(-400, 0) + config.VelocityOffset, config.PayloadMass, "Satellites/PPE+HALO.png");

            var fairingLeft = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var fhS1 = new FHS1B5(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2B5(craftDirectory, DVector2.Zero, DVector2.Zero, 11.3);

            var fhLeftBooster = new FHBoosterB5(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBoosterB5(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            demoSat.AddChild(fhS2);
            fhS2.SetParent(demoSat);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                demoSat, fhS2, fhLeftBooster, fhS1, fhRightBooster, fairingLeft, fairingRight
            };
        }

        public static List<ISpaceCraft> BuildAutoLandingTest(IMassiveBody planet, MissionConfig payload, string craftDirectory)
        {
            var f9 = new F9S1(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 7000),
                planet.Velocity + new DVector2(-400, 400), 4500);

            return new List<ISpaceCraft>
            {
                f9,
            };
        }

        private static List<ISpaceCraft> BuildBFRCrew(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new BFS(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var booster = new BFR(craftDirectory, DVector2.Zero, DVector2.Zero);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildBFR300Crew(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new BFS300(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var booster = new BFR300(craftDirectory, DVector2.Zero, DVector2.Zero);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildBFR19Crew(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new BFS300(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var booster = new BFR19(craftDirectory, DVector2.Zero, DVector2.Zero);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildBfs(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            // inclination 53°
            var ship = new BFS(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity + new DVector2(-277, 0), config.PayloadMass, 1100000);
            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildMiniBfs(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            // inclination 28.5°
            var ship = new MiniBFS(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 75000);
            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildBfs300(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            // inclination 53°
            // var ship = new BFS300(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity + new DVector2(-277, 0), config.PayloadMass, 1100000);
            var ship = new BFS300(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity, 0, 1100000);
            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildStarshipP2P(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            // inclination 28.5°
            var ship = new StarshipP2P(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity, config.PayloadMass, 1100000);
            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildStarship2P2P(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            // inclination 28.5°
            var ship = new Starship2P2P(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity, config.PayloadMass, 1100000);
            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildStarshipTLI(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            double offset = 1.5;
            double perigeeAngle = offset + Math.PI / 2;
            double perigeeAltitude = 166000;
            double posX = Math.Cos(perigeeAngle) * (planet.SurfaceRadius + perigeeAltitude);
            double posY = Math.Sin(perigeeAngle) * (planet.SurfaceRadius + perigeeAltitude);
            double velPerigee = 7810;
            double velX = Math.Sin(perigeeAngle - 2.0 * offset) * velPerigee;
            double velY = Math.Cos(perigeeAngle - 2.0 * offset) * velPerigee;

            var ship = new Starship(craftDirectory, planet.Position + new DVector2(posX, posY), planet.Velocity + new DVector2(velX, velY), config.PayloadMass, 450000);
            return new List<ISpaceCraft>
         {
               ship
         };
        }

        private static List<ISpaceCraft> BuildStarshipTMI(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            double offset = -0.99540882;
            double perigeeAngle = offset + Math.PI / 2;
            double perigeeAltitude = 166000;
            double posX = Math.Cos(perigeeAngle) * (planet.SurfaceRadius + perigeeAltitude);
            double posY = Math.Sin(perigeeAngle) * (planet.SurfaceRadius + perigeeAltitude);
            double velPerigee = 10300;
            double velX = Math.Sin(perigeeAngle - 2.0 * offset) * velPerigee;
            double velY = Math.Cos(perigeeAngle - 2.0 * offset) * velPerigee;

            var ship = new Starship(craftDirectory, planet.Position + new DVector2(posX, posY), planet.Velocity + new DVector2(velX, velY), config.PayloadMass, 1100000);
            // inclination 28.5°
            //var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, planet.SurfaceRadius + 166000.0), planet.Velocity + new DVector2(10275, 0), config.PayloadMass, 1100000);
            return new List<ISpaceCraft>
         {
               ship
         };
        }

        private static List<ISpaceCraft> BuildBfsLeo(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var carousel = new Carousel(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius + 300000.0, 0), planet.Velocity + new DVector2(0, -7730), config.PayloadMass);

            var ship = new BFS(craftDirectory, DVector2.Zero, DVector2.Zero, 0, 1100000);
            //var ship = new BFS(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius + 300000.0, 0),
            //    planet.Velocity + new DVector2(0, -7730), config.PayloadMass, 997800);

            carousel.AddChild(ship);
            ship.SetParent(carousel);

            return new List<ISpaceCraft>
            {
                carousel, ship
            };
        }

        private static List<ISpaceCraft> BuildBfsEarthEdl(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new BFS300(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 166000.0),
                planet.Velocity + new DVector2(-7809, 0), config.PayloadMass, 30000);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildBfsMarsReturnEdl(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new BFS300(craftDirectory, planet.Position + new DVector2(0, planet.SurfaceRadius + 166000.0),
                planet.Velocity + new DVector2(12500, -1634), config.PayloadMass, 30000); // -45° AoA
                                                                                          //planet.Velocity + new DVector2(12500, -1740), config.PayloadMass, 30000); // 80° AoA

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildBfsMarsReturnSkipEntry(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new BFS300(craftDirectory, planet.Position + new DVector2(0, planet.SurfaceRadius + 166000.0),
                planet.Velocity + new DVector2(12500, -2150), config.PayloadMass, 30000); // 45° AoA

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildBfsMarsTEI(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new BFS300(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius, 0),
                planet.Velocity + new DVector2(0, 0), config.PayloadMass);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildITSCrew(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ITSShip(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 0);

            var booster = new ITSBooster(craftDirectory, DVector2.Zero, DVector2.Zero);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildITSTanker(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var tanker = new ITSTanker(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 0);

            return new List<ISpaceCraft>
            {
                tanker
            };
        }

        private static List<ISpaceCraft> BuildItsEDL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ITSShip(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 150000),
                                  planet.Velocity + new DVector2(-7400, 700), config.PayloadMass, 0);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildNewGlenn(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var satellite = new NGSatellite(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);

            var fairingLeft = new NGFairing(craftDirectory, satellite.Position, DVector2.Zero, true);
            var fairingRight = new NGFairing(craftDirectory, satellite.Position, DVector2.Zero, false);

            satellite.AttachFairings(fairingLeft, fairingRight);

            var S1 = new NGS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var S2 = new NGS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            satellite.AddChild(S2);
            S2.SetParent(satellite);
            S2.AddChild(S1);
            S1.SetParent(S2);

            return new List<ISpaceCraft>
            {
                satellite, S2, S1, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildScaledBFR(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ScaledBFS(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 670000);

            var booster = new ScaledBFR(craftDirectory, DVector2.Zero, DVector2.Zero);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildScaledBfrGto(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var carousel = new Carousel(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);

            var ship = new ScaledBFS(craftDirectory, DVector2.Zero, DVector2.Zero, 0, 823000);

            var booster = new ScaledBFR(craftDirectory, DVector2.Zero, DVector2.Zero);

            carousel.AddChild(ship);
            ship.SetParent(carousel);
            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                carousel, ship, booster
            };
        }

        private static List<ISpaceCraft> BuildScaledBfsTLI(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ScaledBFS(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius + 300000.0, 0),
                planet.Velocity + new DVector2(0, -7730), config.PayloadMass, 997800);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildScaledBfsLL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ScaledBFS(craftDirectory, planet.Position + new DVector2(-planet.SurfaceRadius - 160000.0, 0),
                planet.Velocity + new DVector2(0, -1609), config.PayloadMass, 315000);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildScaledBfsTEI(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ScaledBFS(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius, 0),
                planet.Velocity + new DVector2(0, 0), config.PayloadMass, 136600);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildScaledBfsEDL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ScaledBFS(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 150000),
                                  planet.Velocity + new DVector2(-10800, 1161.2), config.PayloadMass, 20000);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildStarHopper(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new StarHopper(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass, config.PropellantMass);

            return new List<ISpaceCraft> { ship };
        }


        private static List<ISpaceCraft> BuildStarKicker(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            //double offset = 0.3334;
            double offset = 0.33325;
            double perigeeAngle = offset + Math.PI / 2;
            double perigeeAltitude = 166000;
            double posX = Math.Cos(perigeeAngle) * (planet.SurfaceRadius + perigeeAltitude);
            double posY = Math.Sin(perigeeAngle) * (planet.SurfaceRadius + perigeeAltitude);
            double velPerigee = 10700;
            double velX = Math.Sin(perigeeAngle - 2.0 * offset) * velPerigee;
            double velY = Math.Cos(perigeeAngle - 2.0 * offset) * velPerigee;

            var ionSat = new StarlinkSat(craftDirectory, planet.Position + new DVector2(posX, posY), planet.Velocity + new DVector2(velX, velY), 0);
            var ship = new StarKicker(craftDirectory, DVector2.Zero, DVector2.Zero);

            ionSat.AddChild(ship);
            ship.SetParent(ionSat);

            return new List<ISpaceCraft> { ionSat, ship };
        }

        private static List<ISpaceCraft> BuildSLS(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new EuropaClipper(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset,
                                      planet.Velocity + new DVector2(-400, 0) + config.VelocityOffset, config.PayloadMass);

            var fairingLeft = new SLS5mFairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new SLS5mFairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var slsS2 = new ICPS(craftDirectory, DVector2.Zero, DVector2.Zero, 9.9);
            var slsS1 = new SLSS1(craftDirectory, DVector2.Zero, DVector2.Zero);

            var slsLeftBooster = new SLSBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var slsRightBooster = new SLSBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            demoSat.AddChild(slsS2);
            slsS2.SetParent(demoSat);
            slsS2.AddChild(slsS1);
            slsS1.SetParent(slsS2);
            slsS1.AddChild(slsLeftBooster);
            slsS1.AddChild(slsRightBooster);
            slsLeftBooster.SetParent(slsS1);
            slsRightBooster.SetParent(slsS1);

            return new List<ISpaceCraft>
            {
                demoSat, slsS2, slsLeftBooster, slsS1, slsRightBooster, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildSLSOrion(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var orion = new Orion(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius, 0) + config.PositionOffset,
                          planet.Velocity + new DVector2(0, -400) + config.VelocityOffset, config.PayloadMass, 0);

            var las = new LAS(craftDirectory, orion.Position, DVector2.Zero);
            orion.AttachLAS(las);

            var esm = new ESM(craftDirectory, DVector2.Zero, DVector2.Zero);
            var icps = new ICPS(craftDirectory, DVector2.Zero, DVector2.Zero, 8.2);
            var slsS1 = new SLSS1(craftDirectory, DVector2.Zero, DVector2.Zero);

            var slsLeftBooster = new SLSBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var slsRightBooster = new SLSBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            orion.AddChild(esm);
            esm.SetParent(orion);
            esm.AddChild(icps);
            icps.SetParent(esm);
            icps.AddChild(slsS1);
            slsS1.SetParent(icps);
            slsS1.AddChild(slsLeftBooster);
            slsS1.AddChild(slsRightBooster);
            slsLeftBooster.SetParent(slsS1);
            slsRightBooster.SetParent(slsS1);

            return new List<ISpaceCraft>
            {
                orion, esm, icps, slsLeftBooster, slsS1, slsRightBooster, las
            };
        }

        private static List<ISpaceCraft> BuildSaturnV(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var commandModule = new CM(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);
            var les = new LES(craftDirectory, commandModule.Position, DVector2.Zero);
            commandModule.AttachLAS(les);

            var serviceModule = new SM(craftDirectory, DVector2.Zero, DVector2.Zero);
            var lunarModule = new LM(craftDirectory, DVector2.Zero, DVector2.Zero);

            var sIVB = new SIVB(craftDirectory, DVector2.Zero, DVector2.Zero);
            var sII = new SII(craftDirectory, DVector2.Zero, DVector2.Zero);
            var interstage = new Saturn.Interstage(craftDirectory, DVector2.Zero, DVector2.Zero);
            var sIC = new SIC(craftDirectory, DVector2.Zero, DVector2.Zero);

            commandModule.AddChild(serviceModule);
            serviceModule.SetParent(commandModule);
            serviceModule.AddChild(lunarModule);
            lunarModule.SetParent(serviceModule);
            lunarModule.AddChild(sIVB);
            sIVB.SetParent(lunarModule);
            sIVB.AddChild(sII);
            sII.SetParent(sIVB);
            sII.AddChild(interstage);
            interstage.SetParent(sII);
            interstage.AddChild(sIC);
            sIC.SetParent(interstage);

            return new List<ISpaceCraft>
            {
                commandModule, serviceModule, lunarModule, sIVB, sII, interstage, sIC, les
            };
        }

        private static List<ISpaceCraft> BuildStarship3SuperHeavy7(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 300000, 100000, 3);

            var booster = new SuperHeavy(craftDirectory, DVector2.Zero, DVector2.Zero, 700000, 200000, 7);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildStarship3SuperHeavy13(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 600000, 120000, 3);

            var booster = new SuperHeavy(craftDirectory, DVector2.Zero, DVector2.Zero, 1600000, 230000, 13);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildStarship3SuperHeavy19(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 700000, 100000, 3);

            var booster = new SuperHeavy(craftDirectory, DVector2.Zero, DVector2.Zero, 2200000, 230000, 19);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildStarshipSuperHeavy25(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var booster = new SuperHeavy(craftDirectory, DVector2.Zero, DVector2.Zero, 2200000, 230000, 25);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildStarshipSuperHeavy28(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var booster = new SuperHeavy(craftDirectory, DVector2.Zero, DVector2.Zero, 3300000, 230000, 28, 210, 300);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildStarshipSuperHeavy31(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var booster = new SuperHeavy(craftDirectory, DVector2.Zero, DVector2.Zero, 3300000, 230000, 31, 250, 300);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildStarshipSuperHeavy37(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var booster = new SuperHeavy(craftDirectory, DVector2.Zero, DVector2.Zero, 3300000);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildStarshipSuperHeavy360(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 1200000, 105000);

            var booster = new SuperHeavy(craftDirectory, DVector2.Zero, DVector2.Zero, 3300000, 190000, 37, 200, 250);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildStarshipSuperHeavy430(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 1200000, 105000);

            var booster = new SuperHeavy(craftDirectory, DVector2.Zero, DVector2.Zero, 3300000, 190000, 37, 250, 300);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildStarshipMk1(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new StarshipMk1(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                //planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 160000);
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 60000);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildStarshipSN4(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new StarshipSN4(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 60000);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildStarshipSN8(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new Starship(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, config.PropellantMass, 120000, 3);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildStarshipSuperHeavyMk1(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new StarshipMk1(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 400000);

            var booster = new SuperHeavyMk1(craftDirectory, DVector2.Zero, DVector2.Zero, 2536000);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }
    }
}
