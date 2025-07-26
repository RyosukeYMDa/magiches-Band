namespace TechC.MagichesBand.Core
{
    /// <summary>
    /// サウンド再生時に指定する音の種類を列挙する
    /// BGMや効果音などを種類ごとに分類して扱いやすくするために使用する
    /// </summary>
    public enum SoundType
    {
        TitleBGM, // タイトル画面で流れるBGM
        SlimeBGM, // スライム戦のバトルBGM
        DragonBGM, // ドラゴン戦のバトルBGM
        BossBGM, // 通常のボス戦BGM
        Boss2BGM, // ボス第二形態戦用BGM
        EndingRollBGM, // エンディングロール表示中のBGM
        EndingBGM, // エンディングロール後のBGM
        StatusUp, // ステータス上昇時の効果音 攻撃力や防御力アップ演出に使用
        PlayerFootstep, // プレイヤーの足音 フィールド移動時に使用
        ButtonNavi, // UI操作時のカーソル移動音に使用
        ButtonSelect, // メニュー選択決定時の音
        ButtonCancel, // メニューキャンセル時の音
        MenuSlide, // メニュー画面がスライドする音
        AreaMovement, // 別エリアやマップへの移動時に使う遷移音
        Explosion, // 爆発系の音
        Shoot, // 弾の発射音
        Damage, // ダメージを受けた時の音
        ItemGet, // アイテムを取得した際の音
        Potion, // HPを回復するアイテム使用時の音
        MPotion, // MPを回復するアイテム使用時の音
        Charge, //体当たりの音
        Fire, // 炎系の魔法の音
        Bite, // 噛みつき攻撃の音
        Breath,  // ブレス系の音
        Rain, // 雨攻撃の音
        Electricity, // 電気魔法の音
        Thunder, // 落雷系魔法の音
        BlackHole, //闇系魔法の音
        Defeated, // 敗北音
        Critical //Critical音
    }
}