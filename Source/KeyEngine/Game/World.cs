using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;

namespace KeyEngine.Game
{
    public static class World
    {
        private static EntityStore m_EntityStore = new EntityStore();

        public static void Update()
        {
            foreach (var scripts in m_EntityStore.EntityScripts)
            {
                foreach (var script in scripts)
                {
                    script.Update();
                }
            }
        }

        public static T AddScriptComponent<T>(Entity entity) where T : ScriptComponent, new()
        {
            var scriptComponent = entity.AddScript(new T());
            return scriptComponent;
        }
    }
}
