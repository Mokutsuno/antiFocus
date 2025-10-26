// File: ICountdownTimer.cs
// Purpose: 抽象的なカウントダウンタイマーの契約を定義する
// Author: Mokutsuno Project (AntiFocusTimer)
/*
using System;

namespace AntiFocusTimer.Core
{
    /// <summary>
    /// カウントダウンタイマーの基本インターフェース。
    /// 残り時間の管理、開始・停止・再開などの操作を定義します。
    /// </summary>
    public interface ICountdownTimer
    {
        /// <summary>
        /// 指定時間でカウントダウンを開始します。
        /// </summary>
        /// <param name="duration">タイマーの総時間</param>
        /// <param name="policy">時間終了時の動作ポリシー（例: 強制停止など）</param>
        void Start(Duration duration, ICyclePolicy policy);

        /// <summary>
        /// タイマーを一時停止します。
        /// </summary>
        void Pause();

        /// <summary>
        /// タイマーを再開します。
        /// </summary>
        void Resume();

        /// <summary>
        /// タイマーを中断・リセットします。
        /// </summary>
        void Cancel();

        /// <summary>
        /// 残り時間を手動で設定します。
        /// </summary>
        /// <param name="remaining">新しい残り時間</param>
        void SetRemainingTime(Duration remaining);

        /// <summary>
        /// 現在の残り時間を取得します。
        /// </summary>
        /// <returns>残り時間</returns>
        Duration GetRemainingTime();

        /// <summary>
        /// タイマーの進捗を外部へ通知する Publisher。
        /// IProgressSubscriber&lt;ProgressInfo&gt; が購読できます。
        /// </summary>
        IProgressPublisher<ProgressInfo> ProgressPublisher { get; }

        /// <summary>
        /// 現在のタイマー状態を取得します。
        /// </summary>
        TimerState State { get; }
    }
}
*/