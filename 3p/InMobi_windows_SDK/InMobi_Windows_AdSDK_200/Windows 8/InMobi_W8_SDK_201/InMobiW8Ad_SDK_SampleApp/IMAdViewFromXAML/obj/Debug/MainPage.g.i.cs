﻿

#pragma checksum "D:\Final\W8-SDK-IMAdNetwork\src\Release\InMobi_W8_SDK_201\InMobiW8Ad_SDK_SampleApp\IMAdViewFromXAML\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7D6FCCB2DDE8C323091F22958B115A7D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMAdViewFromXAML
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::InMobi.W8.AdSDK.IMAdView AdView1; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button btnLoadAd; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button btnViewInterstitialAd; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///MainPage.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            AdView1 = (global::InMobi.W8.AdSDK.IMAdView)this.FindName("AdView1");
            btnLoadAd = (global::Windows.UI.Xaml.Controls.Button)this.FindName("btnLoadAd");
            btnViewInterstitialAd = (global::Windows.UI.Xaml.Controls.Button)this.FindName("btnViewInterstitialAd");
        }
    }
}



