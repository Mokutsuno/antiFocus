// File: ICountdownTimer.cs
// Purpose: ���ۓI�ȃJ�E���g�_�E���^�C�}�[�̌_����`����
// Author: Mokutsuno Project (AntiFocusTimer)
/*
using System;

namespace AntiFocusTimer.Core
{
    /// <summary>
    /// �J�E���g�_�E���^�C�}�[�̊�{�C���^�[�t�F�[�X�B
    /// �c�莞�Ԃ̊Ǘ��A�J�n�E��~�E�ĊJ�Ȃǂ̑�����`���܂��B
    /// </summary>
    public interface ICountdownTimer
    {
        /// <summary>
        /// �w�莞�ԂŃJ�E���g�_�E�����J�n���܂��B
        /// </summary>
        /// <param name="duration">�^�C�}�[�̑�����</param>
        /// <param name="policy">���ԏI�����̓���|���V�[�i��: ������~�Ȃǁj</param>
        void Start(Duration duration, ICyclePolicy policy);

        /// <summary>
        /// �^�C�}�[���ꎞ��~���܂��B
        /// </summary>
        void Pause();

        /// <summary>
        /// �^�C�}�[���ĊJ���܂��B
        /// </summary>
        void Resume();

        /// <summary>
        /// �^�C�}�[�𒆒f�E���Z�b�g���܂��B
        /// </summary>
        void Cancel();

        /// <summary>
        /// �c�莞�Ԃ��蓮�Őݒ肵�܂��B
        /// </summary>
        /// <param name="remaining">�V�����c�莞��</param>
        void SetRemainingTime(Duration remaining);

        /// <summary>
        /// ���݂̎c�莞�Ԃ��擾���܂��B
        /// </summary>
        /// <returns>�c�莞��</returns>
        Duration GetRemainingTime();

        /// <summary>
        /// �^�C�}�[�̐i�����O���֒ʒm���� Publisher�B
        /// IProgressSubscriber&lt;ProgressInfo&gt; ���w�ǂł��܂��B
        /// </summary>
        IProgressPublisher<ProgressInfo> ProgressPublisher { get; }

        /// <summary>
        /// ���݂̃^�C�}�[��Ԃ��擾���܂��B
        /// </summary>
        TimerState State { get; }
    }
}
*/