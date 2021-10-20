using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components;
using OpenGL_Game.GameEngine.Pathing;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenGL_Game.Systems;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Components
{
    enum AI_STATE
    {
        RANDOM_PATH,
        CHASE
    }

    class ComponentAIController : ComponentController
    {
        ComponentTransform target;
        float viewDist = 4.0f;
        Vector3 viewDir = new Vector3(0.0f, 0.0f, 3.0f);
        AStarPathfinder pathingModule;
        Vector3 nextLocation = Vector3.Zero;
        AI_STATE state;

        public bool ObstructedVision;

        public ComponentAIController(Entity ai, Entity pTarget)
        {
            target = pTarget.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            transform = ai.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            pathingModule = new AStarPathfinder("GameCode\\map.txt");
            state = AI_STATE.RANDOM_PATH;
        }

        public void Update(SystemAudio audioSystem, float dt)
        {
            if (!ObstructedVision && PlayerVisible())
            {
                state = AI_STATE.CHASE;
                Console.WriteLine("CHASE");
            }
            else
            {
                state = AI_STATE.RANDOM_PATH;
                Console.WriteLine("RANDOM");
            }

            if (state == AI_STATE.RANDOM_PATH)
            {
                if (nextLocation != Vector3.Zero)
                {
                    Vector3 vec = (nextLocation - transform.Position);
                    if (vec.Length > -0.01f && vec.Length < 0.01f)
                    {
                        nextLocation = Vector3.Zero;
                    }
                    transform.Position += (vec.Normalized() * 1.0f) * dt;
                }
                else if (pathingModule.Path == null || pathingModule.Path.Count == 0)
                {
                    pathingModule.GeneratePath(transform.Position, target.Position, GameScene.WorldTranslate);
                }
                else
                {
                    nextLocation = pathingModule.Path[0];
                    pathingModule.Path.RemoveAt(0);
                }
            }

            ObstructedVision = false; //Reset value for next frame.
        }

        private bool PlayerVisible()
        {
            Vector3 toTarget = (target.Position - transform.Position);
            if (toTarget.Length > viewDist) return false;
            toTarget.Normalize();
            float vDotT = Vector3.Dot(toTarget, viewDir.Normalized());
            return vDotT >= 0.707;
        }

        protected override void UpdateView(float dt)
        {
            Vector3 dist = target.Position - transform.Position;
            Vector3 frontDir = Vector3.UnitZ;
            Vector3 toDir = dist.Normalized();
            float rotAngle = (float)Math.Acos(Vector3.Dot(frontDir, toDir));
            Vector3 rotAxis = Vector3.Cross(frontDir, toDir).Normalized();
            transform.Rotation = rotAxis * rotAngle;
        }
    }
}
