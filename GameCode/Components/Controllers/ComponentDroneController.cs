using OpenGL_Game.Components;
using OpenGL_Game.GameEngine.Components.Physics;
using OpenGL_Game.GameEngine.Pathing;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using OpenGL_Game.Systems;
using OpenTK;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.GameCode.Components.Controllers
{
    enum AI_STATE
    {
        NONE,
        WALKING_ON_PATH,
        CHASE,
        GET_TO_NODE,
        GET_NEW_PATH
    }

    class ComponentDroneController : ComponentAIController, IControllerWithView
    {
        private AStarPathfinder pathingModule;
        private ComponentTransform target;
        private ComponentAudio buzzSfx;
        private Vector3 viewDir = new Vector3(0.0f, 0.0f, 1.0f);
        private AI_STATE state;
        private float viewDist = 8.0f;
        public bool ObstructedVision;

        public ComponentDroneController(Entity AI, Entity pTarget, string graphMapTxtPath) : base(AI)
        {
            target = pTarget.FindComponentByType(ComponentTypes.COMPONENT_TRANSFORM) as ComponentTransform;
            buzzSfx = AI.FindComponentByType(ComponentTypes.COMPONENT_AUDIO) as ComponentAudio;
            pathingModule = new AStarPathfinder(graphMapTxtPath);
            state = AI_STATE.NONE;
            speed = 1.0f;

        }

        private void AStarCreatePath()
        {
            pathingModule.GenerateRandomPath(transform.Position, randomFunc);
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

        private bool PlayerVisible()
        {
            Vector3 toTarget = (target.Position - transform.Position);
            if (toTarget.Length > viewDist) return false;
            toTarget.Normalize();
            float vDotT = Vector3.Dot(toTarget, viewDir);
            return vDotT >= 0.707;
        }

        public override void ResetPosition()
        {
            base.ResetPosition();
            state = AI_STATE.GET_TO_NODE;
            pathingModule.ResetPath();
        }

        public override void Update(SystemAudio audioSystem, float dt)
        {
            if (InputManager.StopDrone) { return; }
            if (!ObstructedVision && PlayerVisible())
            {
                state = AI_STATE.CHASE;

                audioSystem.PlaySound(buzzSfx, true);
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
                else if (state == AI_STATE.CHASE || nextLocation == Vector3.Zero)
                    state = AI_STATE.GET_TO_NODE;
            }

            switch (state)
            {
                case AI_STATE.WALKING_ON_PATH:
                    AStarWalk();
                    break;
                case AI_STATE.GET_NEW_PATH:
                    AStarCreatePath();
                    break;
                case AI_STATE.GET_TO_NODE:
                    audioSystem.StopSound(buzzSfx);
                    nextLocation = pathingModule.GetClosestNode(transform.Position) + GameScene.WorldTranslate;
                    break;
                case AI_STATE.CHASE:
                    nextLocation = target.Position;
                    break;
            }

            MoveToNextLocation(dt);

            UpdateView(dt);
            ObstructedVision = false; //Reset value for next frame.
        }

        public void UpdateView(float dt)
        {
            if (nextLocation == Vector3.Zero) return; //Get NaN if normalize V3.Zero

            Vector3 vec = (nextLocation - transform.Position);
            if (vec.Length < 1.0f) { return; }
            vec.Normalize();
            if (float.IsNaN(vec.X)) { return; }
            float rotAngle = (float)Math.Acos(Vector3.Dot(viewDir, vec));

            if (rotAngle < 0.015f || float.IsNaN(rotAngle))
            {
                return;
            }
            Vector3 rotAxis = Vector3.Cross(viewDir, vec);
            transform.Rotation += rotAxis.Normalized() * rotAngle;
            viewDir = vec;
        }
    }
}
