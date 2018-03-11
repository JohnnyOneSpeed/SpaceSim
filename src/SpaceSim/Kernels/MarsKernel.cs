﻿using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class MarsKernel : SymbolKernel, IMassiveKernel
    {
        // -------- ------ Image ----- --------
        // 01111111 00000000 00000000 00000000 alpha >> 24
        // 00000000 11111111 00000000 00000000 red >> 16
        // 00000000 00000000 11111111 00000000 green >> 8
        // 00000000 00000000 00000000 11111111 blue
        public void Run(int[] image, int resX, int resY, double cX, double cY, double cWidth, double cHeight, double cRot, double sunNormalX, double sunNormalY, double bodyX, double bodyY, double bodyRot)
        {
            int index = get_global_id(0);

            // screen-space coords
            int y = index / resX;
            int x = index - (y * resX);

            // screen-space uvmap
            float u = (float)x / resX;
            float v = (float)y / resY;

            // world-space pixel location
            double worldX = cX + cWidth * u;
            double worldY = cY + cHeight * v;
            double distance = sqrt(worldX * worldX + worldY * worldY);

            if (distance < MARS_RADIUS + MARS_ATMOSPHERE)
            {
                double worldNormalX = worldX / distance;
                double worldNormalY = worldY / distance;

                double sunDotProduct = sunNormalX * worldNormalX + sunNormalY * worldNormalY;

                if (sunDotProduct < 0)
                {
                    sunDotProduct = 0.05f;
                }
                else
                {
                    sunDotProduct = 0.05f + sunDotProduct * 0.95f;
                }

                if (distance < MARS_RADIUS)
                {
                    double ratio = distance / MARS_RADIUS;

                    double worldAngle = atan2(worldNormalX, worldNormalY) + atan2(ratio, 1.1) + bodyRot;

                    double textureFactor = 0.25 * sin(worldAngle * 30) + 0.75;

                    int rComponent = (int)(sunDotProduct * 255 * ratio * textureFactor);
                    int gComponent = (int)(sunDotProduct * 150 * ratio * textureFactor);

                    image[index] = ALPHA | (gComponent << 8) | (rComponent << 16);
                }
                else
                {
                    double ratio = (distance - MARS_RADIUS) / MARS_ATMOSPHERE;

                    int rgComponents = (int)((255 - ratio * 255) * sunDotProduct);

                    image[index] = ALPHA | (rgComponents << 8) | (rgComponents << 16);
                }
            }
        }
    }
}
