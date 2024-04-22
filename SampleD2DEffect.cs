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
    /// �f���G�t�F�N�g
    /// �f���G�t�F�N�g�ɂ͕K��[VideoEffect]������ݒ肵�Ă��������B
    /// </summary>
    [VideoEffect("�_�ł��Ȃ���o��ޏ�", new[] { "�o��ޏ�" }, new string[] { },IsAviUtlSupported =false)]
    internal class SampleD2DVideoEffect : VideoEffectBase
    {
        /// <summary>
        /// �G�t�F�N�g�̖��O
        /// </summary>
        public override string Label => "�_�ł��Ȃ���o��ޏ�";

        [Display(GroupName = "�o��ޏ�", Name = "�o��", Description = "�A�C�e�����o�ꂷ��ۂɃG�t�F�N�g��K�p����")]
        [ToggleSlider]
        public bool IsAppearance { get => isAppearance; set => Set(ref isAppearance, value); }
        bool isAppearance = false;

        [Display(GroupName = "�o��ޏ�", Name = "�ޏ�", Description = "�A�C�e�����ޏꂷ��ۂɃG�t�F�N�g��K�p����")]
        [ToggleSlider]
        public bool IsExit { get => isExit; set => Set(ref isExit, value); }
        bool isExit = false;

        [Display(GroupName = "�o��ޏ�", Name = "���ʎ���", Description = "�G�t�F�N�g���Đ������b��")]
        [TextBoxSlider("F2", "�b", 0.00, 1.00)]
        [DefaultValue(0.30)]
        [Range(0.00, 100.00)]
        public double Duration { get => duration; set => Set(ref duration, value); }
        double duration = 0.00;

        [Display(GroupName = "�o��ޏ�", Name = "�\���Ԋu", Description = "�_�ł���Ԋu")]
        [AnimationSlider("F2", "�b", 0.00, 1.00)]
        public Animation Interval { get; } = new Animation(0.30, 0.00, 100.00);

        [Display(GroupName = "�o��ޏ�", Name = "�t���[���w��", Description = "�J���}��؂�œ����ɂ������t���[�����w�肵�܂�(��F2,6->2��6�t���[���œ���)")]
        [TextEditor(AcceptsReturn = true)]
        public string FrameSelectText { get => frameSelectText; set => Set(ref frameSelectText, value); }
        string frameSelectText = string.Empty;

        [Display(GroupName = "�o��ޏ�", Name = "���]", Description = "�\����ԂƓ�����Ԃ����ւ��܂�")]
        [ToggleSlider]
        public bool BlinkinToggle { get => blinkintoggle; set => Set(ref blinkintoggle, value); }
        bool blinkintoggle = false;

        /*
        [Display(GroupName = "�o��ޏ�", Name = "�f�o�b�O", Description = "�J���}��؂�œ����ɂ������t���[�����w�肵�܂�(��F2,6->2��6�t���[���œ���)")]
        [TextEditor(AcceptsReturn = true)]
        public string Text { get => text; set => Set(ref text, value); }
        string text = string.Empty;
        */

        /// <summary>
        /// ExoFilter���쐬����
        /// </summary>
        /// <param name="keyFrameIndex">�L�[�t���[���ԍ�</param>
        /// <param name="exoOutputDescription">exo�o�͂ɕK�v�Ȋe��p�����[�^�[</param>
        /// <returns></returns>
        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            var fps = exoOutputDescription.VideoInfo.FPS;
            return new[]
            {
                $"_name=�ڂ���\r\n" +
                $"_disable={(IsEnabled ?1:0)}\r\n" +
                $"�c����=0.0\r\n" +
                $"���̋���=0\r\n" +
                $"�T�C�Y�Œ�=0\r\n",
            };
        }

        /// <summary>
        /// �f���G�t�F�N�g���쐬����
        /// </summary>
        /// <param name="devices">�f�o�C�X</param>
        /// <returns>�f���G�t�F�N�g</returns>
        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new SampleD2DVideoEffectProcessor(devices, this);
        }

        /// <summary>
        /// �N���X����IAnimatable�̈ꗗ���擾����
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IAnimatable> GetAnimatables() => new IAnimatable[] { Interval };
    }
}