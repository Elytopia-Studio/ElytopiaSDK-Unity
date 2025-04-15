using Elytopia;

namespace Samples.Elytopia
{
    internal class KidsGamesCheatSheet
    {
        private async void Test()
        {
            //Close WebGL Game
            ElytopiaSDK.Instance.CloseGame();

            //Show Interstitial Ad with a result 
            var result = await ElytopiaSDK.Instance.ShowInterstitialAdAsync();
            switch (result)
            {
                case ResultContext.Success:
                    //Process Success
                    break;
                case ResultContext.Failed:
                    //Process Failed
                    break;
            }
        }
    }
}