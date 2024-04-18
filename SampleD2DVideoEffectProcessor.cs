using BlinkinYMM4Plugin.SampleD2DEffect;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using System.Text.RegularExpressions;
using Windows.Win32.UI.KeyboardAndMouseInput;
using Windows.Security.Cryptography.Core;
namespace BlinkinYMM4Plugin.SampleD2DEffect
{
    internal class SampleD2DVideoEffectProcessor : IVideoEffectProcessor
    {
        readonly SampleD2DVideoEffect item;
        readonly Vortice.Direct2D1.Effects.Opacity opacityEffect;

        public ID2D1Image Output { get; }

        double duration;
        double interval;
        string frameSelectText;
        bool blinkintoggle;
        bool isAppearance;
        bool isExit;

        public SampleD2DVideoEffectProcessor(IGraphicsDevicesAndContext devices, SampleD2DVideoEffect item)
        {
            this.item = item;

            opacityEffect = new Vortice.Direct2D1.Effects.Opacity(devices.DeviceContext);
            Output = opacityEffect.Output;//EffectからgetしたOutputは必ずDisposeする。Effect側ではDisposeされない。
        }

        /// <summary>
        /// エフェクトに入力する映像を設定する
        /// </summary>
        /// <param name="input"></param>
        public void SetInput(ID2D1Image input)
        {
            opacityEffect.SetInput(0, input, true);
        }

        /// <summary>
        /// エフェクトに入力する映像をクリアする
        /// </summary>
        public void ClearInput()
        {
            opacityEffect.SetInput(0, null, true);
        }

        /// <summary>
        /// エフェクトを更新する
        /// </summary>
        /// <param name="effectDescription">エフェクトの描画に必要な各種設定項目</param>
        /// <returns>描画関連の設定項目</returns>
        public DrawDescription Update(EffectDescription effectDescription)
        {
            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;
            double interval = item.Interval.GetValue(frame, length, fps);


            bool isDisplay = true;
            
           
            double duration = item.Duration;
            float durationByFrame = (float)duration * (float)fps;
            bool blinkintoggle = item.BlinkinToggle;
            float visible = (float)1.0;
            float hidden = (float)0.0;
            bool isAppearance = item.IsAppearance;
            bool isExit = item.IsExit;

            //item.Text = $"frame:{frame}\nlength:{length}\nfps:{fps}\nDuration frame:{durationByFrame}";

            int[] frameSelects;

            try
            {
                frameSelects = Regex.Replace(item.FrameSelectText, @"[^\d,]", "").Split(',').Select(x => int.Parse(x)).ToArray();
            }
            catch
            {

                frameSelects = [];
            }

            void Blinkin(int frame)
            {
               
                var seconds = (float)frame / (float)fps;
                if (seconds < duration)
                {
                    if (((seconds < interval) || ((int)Math.Floor(seconds / interval) % 2 == 0)))
                    {
                        //item.Text = $"{seconds.ToString()},{interval.ToString()}";
                        isDisplay = true;
                    }
                    else
                    {
                        isDisplay = false;
                    }
                }


                if (frameSelects.Contains(frame))
                {
                    isDisplay = false;
                }

                if (isDisplay)
                {
                    opacityEffect.Value = blinkintoggle ? hidden : visible;
                }
                else
                {
                    opacityEffect.Value = blinkintoggle ? visible : hidden;
                }

               
            }


            int exitStarFrame = length - (int)durationByFrame;
            //item.Text += $"\nExit Start Frame:{exitStarFrame}";
            if (frame < (int)durationByFrame)
            {
                if (isAppearance)
                {
                    Blinkin(frame);
                }
            }
            else if (frame > exitStarFrame)
            {
                if (isExit)
                {
                    int exitFrame =   frame - exitStarFrame;
                   // item.Text += $"\nExit Frame:{exitFrame}";
                    Blinkin(exitFrame);
                }
            }
            else
            {
                opacityEffect.Value = visible;
            }


            
            this.duration = duration;
            this.interval = interval;
            this.blinkintoggle = blinkintoggle;
            return effectDescription.DrawDescription;
        }


        public void Dispose()
        {
            opacityEffect.SetInput(0, null, true);//Inputは必ずnullに戻す。
            Output.Dispose();//EffectからgetしたOutputは必ずDisposeする。Effect側ではDisposeされない。
            opacityEffect.Dispose();
        }
    }
}