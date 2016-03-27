using System;
using StardewModdingAPI;
using StardewModdingAPI.Inheritance;
using StardewModdingAPI.Events;

namespace SaveAnywhereMod
{
    public class SaveAnywhere : Mod
    {
        public SaveAnywhereConfig Config { get; private set; }
        private bool readyToLoad = false;
        private const string _debuggerInfo = "[SaveAnywhere INFO] ";

        public override void Entry(params object[] objects)
        {
            Config = new SaveAnywhereConfig().InitializeConfig(BaseConfigPath);
            ControlEvents.KeyPressed += ControlEvents_KeyPressed;
            PlayerEvents.LoadedGame += PlayerEvents_LoadedGame;
            GameEvents.UpdateTick += GameEvents_UpdateTick;
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {            
            if (readyToLoad && SGame.player != null && SGame.currentLoader.Current == 100)
            {
                WarpPlayer();
                readyToLoad = false;
            }
                
        }

        private void PlayerEvents_LoadedGame(object sender, EventArgsLoadedGameChanged e)
        {
            Log.Info(_debuggerInfo + "Game loaded.");
            if (e.LoadedGame && Config.LoadOnLoad)
            {
                Log.Info(_debuggerInfo + "Attempting to resume game from recent save data.");
                readyToLoad = true;
            }
        }

        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (e.KeyPressed.ToString().Equals("NumPad0"))
            {
                Save();
            }
            else if (e.KeyPressed.ToString().Equals("NumPad9"))
            {
                WarpPlayer();
            }
        }

        public void ChatListener()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            Config.XPos = SGame.player.getTileX();
            Config.YPos = SGame.player.getTileY();
            Config.SavedLocationName = SGame.player.currentLocation.Name;
            Config.SavedFacingDirection = SGame.player.FacingDirection;
            Config.SavedTimeOfDay = SGame.timeOfDay;
            Config.LoadOnLoad = true;
            Config.WriteConfig();
            Log.Info(_debuggerInfo + "Save Successful.");
            SGame.setGameMode(0);
            //StardewValley.SaveGame.getSaveEnumerator();
        }

        public void WarpPlayer()
        {
            SGame.warpFarmer(Config.SavedLocationName, Config.XPos, Config.YPos, Config.SavedFacingDirection);
            SGame.timeOfDay = Config.SavedTimeOfDay;
            Config.LoadOnLoad = false;
            Config.WriteConfig();
        }
    }

    public class SaveAnywhereConfig : Config
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public string SavedLocationName { get; set; }
        public int SavedFacingDirection { get; set; }
        public int SavedTimeOfDay { get; set; }
        public bool LoadOnLoad { get; set; }

        public override T GenerateDefaultConfig<T>()
        {
            XPos = 0;
            YPos = 0;
            SavedLocationName = "Town";
            SavedFacingDirection = 0;
            LoadOnLoad = false;
            return this as T;
        }
    }
}
