using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace BlinkinYMM4Plugin.SampleD2DEffect
{
    /// <summary>
    /// 映像エフェクト
    /// 映像エフェクトには必ず[VideoEffect]属性を設定してください。
    /// </summary>
    [VideoEffect("点滅しながら登場退場", new[] { "登場退場" }, new string[] { },IsAviUtlSupported =false)]
    internal class SampleD2DVideoEffect : VideoEffectBase
    {
        /// <summary>
        /// エフェクトの名前
        /// </summary>
        public override string Label => "点滅しながら登場退場";

        [Display(GroupName = "登場退場", Name = "登場", Description = "アイテムが登場する際にエフェクトを適用する")]
        [ToggleSlider]
        public bool IsAppearance { get => isAppearance; set => Set(ref isAppearance, value); }
        bool isAppearance = false;

        [Display(GroupName = "登場退場", Name = "退場", Description = "アイテムが退場する際にエフェクトを適用する")]
        [ToggleSlider]
        public bool IsExit { get => isExit; set => Set(ref isExit, value); }
        bool isExit = false;

        [Display(GroupName = "登場退場", Name = "効果時間", Description = "エフェクトが再生される秒数")]
        [TextBoxSlider("F2", "秒", 0.00, 1.00)]
        [DefaultValue(0.30)]
        [Range(0.00, 100.00)]
        public double Duration { get => duration; set => Set(ref duration, value); }
        double duration = 0.00;

        [Display(GroupName = "登場退場", Name = "表示間隔", Description = "点滅する間隔")]
        [AnimationSlider("F2", "秒", 0.00, 1.00)]
        public Animation Interval { get; } = new Animation(0.30, 0.00, 100.00);

        [Display(GroupName = "登場退場", Name = "フレーム指定", Description = "カンマ区切りで透明にしたいフレームを指定します(例：2,6->2と6フレームで透明)")]
        [TextEditor(AcceptsReturn = true)]
        public string FrameSelectText { get => frameSelectText; set => Set(ref frameSelectText, value); }
        string frameSelectText = string.Empty;

        [Display(GroupName = "登場退場", Name = "反転", Description = "表示状態と透明状態を入れ替えます")]
        [ToggleSlider]
        public bool BlinkinToggle { get => blinkintoggle; set => Set(ref blinkintoggle, value); }
        bool blinkintoggle = false;

        /*
        [Display(GroupName = "登場退場", Name = "デバッグ", Description = "カンマ区切りで透明にしたいフレームを指定します(例：2,6->2と6フレームで透明)")]
        [TextEditor(AcceptsReturn = true)]
        public string Text { get => text; set => Set(ref text, value); }
        string text = string.Empty;
        */

        /// <summary>
        /// ExoFilterを作成する
        /// </summary>
        /// <param name="keyFrameIndex">キーフレーム番号</param>
        /// <param name="exoOutputDescription">exo出力に必要な各種パラメーター</param>
        /// <returns></returns>
        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            var fps = exoOutputDescription.VideoInfo.FPS;
            return new[]
            {
                $"_name=ぼかし\r\n" +
                $"_disable={(IsEnabled ?1:0)}\r\n" +
                $"縦横比=0.0\r\n" +
                $"光の強さ=0\r\n" +
                $"サイズ固定=0\r\n",
            };
        }

        /// <summary>
        /// 映像エフェクトを作成する
        /// </summary>
        /// <param name="devices">デバイス</param>
        /// <returns>映像エフェクト</returns>
        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new SampleD2DVideoEffectProcessor(devices, this);
        }

        /// <summary>
        /// クラス内のIAnimatableの一覧を取得する
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IAnimatable> GetAnimatables() => new IAnimatable[] { Interval };
    }
}