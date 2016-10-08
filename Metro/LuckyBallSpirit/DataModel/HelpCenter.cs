using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallSpirit.Controls;
using LuckyBallsData.Reference;
using LuckyBallsData.Util;

namespace LuckyBallSpirit.DataModel
{
    class HelpCenter : HelpManager
    {
        private bool _loaded = false;
        private static HelpCenter _instance = null;
        private HelpViewCtrl _helpViewer = new HelpViewCtrl();

        public bool DataLoaded
        {
            get
            {
                return _loaded;
            }
        }

        public void Show(int topicID)
        {
            // Load content if not loaded.
            if (DataLoaded)
            {
                // get the topic by id.
                Topic topic = GetTopic(topicID);
                if (topic != null)
                {
                    _helpViewer.Show(topic);
                }
            }
        }

        public static HelpCenter Instance()
        {
            if (_instance == null)
            {
                _instance = new HelpCenter();
            }

            return _instance;
        }

        public async Task<bool> Load()
        {
            if (_loaded)
                return false;

            DBXmlDocument xml = await DataUtil.GetHelpXml(LBDataManager.GetInstance().GetSyncContext());
            if (xml != null)
            {
                try
                {
                    DBXmlNode root = xml.Root();
                    Read(root);

                    _loaded = true;
                }
                catch
                {
                }                
            }

            return false;
        }
    }
}
