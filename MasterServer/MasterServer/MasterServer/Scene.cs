using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer
{
    class Scene
    {
        public static Scene Instance;

        public Scene()
        {
            Instance = this;
        }

        private Dictionary<string, ScenePlayer> SPDic = new Dictionary<string, ScenePlayer>();

        //根据ID获取ScenePlayer
        private ScenePlayer GetScenePlayer(string id)
        {
            ScenePlayer sp;
            if (SPDic.TryGetValue(id, out sp))
            {
                return sp;
            }
            return null;
        }

        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="id"></param>
        public void AddPlayer(string id)
        {
            lock (SPDic)
            {
                SPDic[id] = new ScenePlayer();
            }
        }

        public void DelPlayer(string id)
        {
            lock (SPDic)
            {
                SPDic.Remove(id);
            }

            //???
            
        }
    }
}