using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components;
using OpenGL_Game.GameEngine.Components.Physics;
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
        WALKING_ON_PATH,
        CHASE,
        GET_TO_NODE,
        GET_NEW_PATH
    }

    class ComponentAIController : ComponentController
    {
        private AStarPathfinder pathingModule;
        private Entity entity;
        private ComponentTransform target;

        private Vector3 nextLocation = Vector3.Zero;
        private Vector3 viewDir = new Vector3(0.0f, 0.0f, 1.0f);
        private AI_STATE state;

        private float viewDist = 4.0f;
        
        public bool ObstructedVision;

        public ComponentAIController(Entity ai, Entity pTarget)
        {
            entity = ai;
            target = pTarget.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            transform = ai.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            pathingModule = new AStarPathfinder("GameCode\\map.txt");
        }

        public void Update(SystemAudio audioSystem, float dt)
        {
            if (false)
            //if (!ObstructedVision && PlayerVisible())
            {
                state = AI_STATE.CHASE;
                if (pathingModule.IsOnPath())
                    pathingModule.ResetPath();
            }
            else
            {
                AStarPathfinder.WorldTranslate = GameScene.WorldTranslate;
                if (pathingModule.IsOnPath())
                {
                    state = AI_STATE.WALKING_ON_PATH;
                }
                else if (pathingModule.IsOnNode(transform.Position))
                {
                    state = AI_STATE.GET_NEW_PATH;
                }
                else if (nextLocation == Vector3.Zero)
                    state = AI_STATE.GET_TO_NODE;
            }

            switch (state)
            {
                case AI_STATE.WALKING_ON_PATH:
                    Console.WriteLine("Walking To Next Node");
                    AStarWalk();
                    break;
                case AI_STATE.GET_NEW_PATH:
                    Console.WriteLine("Creating new path");
                    AStarCreatePath();
                    break;
                case AI_STATE.GET_TO_NODE:
                    Console.WriteLine("Moving to closest node");
                    pathingModule.GetClosestNode(transform.Position);
                    break;
                case AI_STATE.CHASE:
                    //Console.WriteLine("Chasing");
                    //nextLocation = target.Position;
                    break;
            }

            MoveToNextLocation(dt);
            
            UpdateView(dt);
            ObstructedVision = false; //Reset value for next frame.
        }

        private void AStarCreatePath()
        {
            pathingModule.GenerateRandomPath(transform.Position);
        }

        private void AStarWalk()
        {
            Vector3 vec = (nextLocation - transform.Position);
            if (vec.Length > -0.01f && vec.Length < 0.01f) //we're at location, get next location
            {
                nextLocation = Vector3.Zero;
                pathingModule.Path.RemoveAt(0);
            }
            else
            {
                nextLocation = pathingModule.Path[0];
            }
        }

        private void MoveToNextLocation(float dt)
        {
            if (nextLocation != Vector3.Zero) //we have next destination, move to it
            {
                Vector3 vec = (nextLocation - transform.Position);
                transform.Position += (vec.Normalized() * 1.0f) * dt;
            }
        }

        private bool PlayerVisible()
        {
            Vector3 toTarget = (target.Position - transform.Position);
            if (toTarget.Length > viewDist) return false;
            toTarget.Normalize();
            float vDotT = Vector3.Dot(toTarget, viewDir);
            return vDotT >= 0.707;
        }

        protected override void UpdateView(float dt)
        {
            if (nextLocation == Vector3.Zero) return; //Get NaN if normalize V3.Zero

            Vector3 vec = (nextLocation - transform.Position).Normalized();
            if (float.IsNaN(vec.X)) { return; }
            if (vec.Length < 1.0f) { return; }
            float rotAngle = (float)Math.Acos(Vector3.Dot(viewDir, vec));

            if (rotAngle < 0.01f || float.IsNaN(rotAngle))
            { 
                return;
            }
            Vector3 rotAxis = Vector3.Cross(viewDir, vec);
            transform.Rotation += rotAxis.Normalized() * rotAngle;
            viewDir = vec;
        }
    }
}
