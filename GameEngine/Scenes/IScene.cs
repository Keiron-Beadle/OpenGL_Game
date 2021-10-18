using OpenTK;

namespace OpenGL_Game.Scenes
{
    public enum SceneType
    {
        NULL_SCENE,
        GAME_SCENE,
        MAIN_MENU_SCENE,
        GAME_OVER_SCENE,
    }

    interface IScene
    {
        void Render(FrameEventArgs e);
        void Update(FrameEventArgs e);
        void Close();
    }
}
