using OpenGL_Game.Objects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Managers
{
    class LightManager
    {
        static int pointLightIndex = 0;
        static int spotLightIndex = 0;
        static int NUMOFPOINTLIGHTS = 8;
        static int NUMOFSPOTLIGHTS = 4;
        static float maxDir = 2.0f;
        public static PointLight[] PointLights = new PointLight[NUMOFPOINTLIGHTS];
        public static SpotLight[] SpotLights = new SpotLight[NUMOFSPOTLIGHTS];
        static List<Vector3> targetDirections = new List<Vector3>();
        static List<Vector3> velocityOfSpotlights = new List<Vector3>();
        static Random rnd = new Random();
        const float speed = 0.2f;

        public static void AddLight(PointLight pointLight)
        {
            PointLights[pointLightIndex] = pointLight;

            pointLightIndex++;
            if (pointLightIndex >= PointLights.Length)
                pointLightIndex = 0;
        }

        public static void AddLight(SpotLight spotLight)
        {
            SpotLights[spotLightIndex] = spotLight;
            spotLightIndex++;
            if (spotLightIndex >= SpotLights.Length)
                spotLightIndex = 0;
            float ranX = (float)rnd.NextDouble() * maxDir - maxDir/2;
            float ranZ = (float)rnd.NextDouble() * maxDir - maxDir/2;
            targetDirections.Add(new Vector3(ranX,0,ranZ));
            velocityOfSpotlights.Add(targetDirections.Last() - spotLight.coneDirection);
        }

        public static void Update(float dt)
        {
            for (int i = 0; i < SpotLights.Length; i++)
            {
                if (targetDirections[i] == -Vector3.UnitY &&
                    (targetDirections[i].X - SpotLights[i].coneDirection.X) <= 0.001f &&
                    (targetDirections[i].Z - SpotLights[i].coneDirection.Z) <= 0.001f)
                {
                    float ranX = (float)rnd.NextDouble() * maxDir - maxDir / 2;
                    float ranZ = (float)rnd.NextDouble() * maxDir - maxDir / 2;
                    targetDirections[i] = new Vector3(ranX, 0, ranZ);
                    velocityOfSpotlights[i] = targetDirections[i] - SpotLights[i].coneDirection;
                }
                else if ((targetDirections[i].X - SpotLights[i].coneDirection.X) <= 0.001f &&
                    (targetDirections[i].Z - SpotLights[i].coneDirection.Z) <= 0.001)
                {
                    targetDirections[i] = -Vector3.UnitY;
                    velocityOfSpotlights[i] = targetDirections[i] - SpotLights[i].coneDirection;
                }
                SpotLights[i].coneDirection += velocityOfSpotlights[i] * speed * dt;
            }
        }

    }
}
